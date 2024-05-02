using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIMatchmakingBox : UIMenu
    {
        private static MatchmakingBoxCore _core = new MatchmakingBoxCore();
        private Sprite _frame;
        private SpriteMap _matchmakingSignal;
        private List<SpriteMap> _matchmakingStars = new List<SpriteMap>();
        private BitmapFont _font;
        private FancyBitmapFont _fancyFont;
        private float _scroll;
        private Lobby _tryHostingLobby;
        protected Lobby _tryConnectLobby;
        private int _tries;
        private float _tryConnectTimeout;
        private bool _quit;
        private SpriteMap _signalCrossLocal;
        private SpriteMap _signalCrossNetwork;
        private List<BlacklistServer> _failedAttempts = new List<BlacklistServer>();
        public List<BlacklistServer> _permenantBlacklist = new List<BlacklistServer>();
        private List<string> _statusList = new List<string>();
        protected List<string> _newStatusList = new List<string>();
        private float _newStatusWait = 1f;
        private float _tryHostingWait;
        private UIMenu _openOnClose;
        private float _stateWait;
        private MatchmakingState _pendingState;
        private int searchTryIndex;
        private bool triedHostingAlready;
        protected bool playMusic = true;
        private Level _currentLevel;
        private Dictionary<Profile, Team> _teamProfileLinks = new Dictionary<Profile, Team>();
        protected bool _searchingIsOver;
        protected bool _continueSearchOnFail = true;
        private int _totalLobbiesFound = -1;
        private int _totalInGameLobbies;
        private int _triesSinceSearch;
        private int _connectTimeout;
        public static bool errorConnectingToGame = false;
        private long _globalKills;
        private float _dots;
        protected string _caption = "LOOKING";

        public static MatchmakingBoxCore core
        {
            get => _core;
            set => _core = value;
        }

        public List<MatchmakingPlayer> matchmakingProfiles
        {
            get => core.matchmakingProfiles;
            set => core.matchmakingProfiles = value;
        }

        public UIMatchmakingBox(UIMenu openOnClose, float xpos, float ypos, float wide = -1f, float high = -1f)
          : base("", xpos, ypos, wide, high)
        {
            _openOnClose = openOnClose;
            Graphics.fade = 1f;
            _frame = new Sprite("online/matchmaking");
            _frame.CenterOrigin();
            _font = new BitmapFont("biosFontUI", 8, 7);
            _fancyFont = new FancyBitmapFont("smallFont");
            _matchmakingSignal = new SpriteMap("online/matchmakingSignal", 4, 9);
            _matchmakingSignal.CenterOrigin();
            SpriteMap spriteMap1 = new SpriteMap("online/matchmakingStar", 7, 7);
            spriteMap1.AddAnimation("flicker", 0.08f, true, 0, 1, 2, 1);
            spriteMap1.SetAnimation("flicker");
            spriteMap1.CenterOrigin();
            _signalCrossLocal = new SpriteMap("online/signalCross", 5, 5);
            _signalCrossLocal.AddAnimation("idle", 0.12f, true, new int[1]);
            _signalCrossLocal.AddAnimation("flicker", 0.12f, false, 1, 2, 3);
            _signalCrossLocal.SetAnimation("idle");
            _signalCrossLocal.CenterOrigin();
            _signalCrossNetwork = new SpriteMap("online/signalCross", 5, 5);
            _signalCrossNetwork.AddAnimation("idle", 0.12f, true, new int[1]);
            _signalCrossNetwork.AddAnimation("flicker", 0.12f, false, 1, 2, 3);
            _signalCrossNetwork.SetAnimation("idle");
            _signalCrossNetwork.CenterOrigin();
            _matchmakingStars.Add(spriteMap1);
            SpriteMap spriteMap2 = new SpriteMap("online/matchmakingStar", 7, 7);
            spriteMap2.AddAnimation("flicker", 0.11f, true, 0, 1, 2, 1);
            spriteMap2.SetAnimation("flicker");
            spriteMap2.CenterOrigin();
            _matchmakingStars.Add(spriteMap2);
            SpriteMap spriteMap3 = new SpriteMap("online/matchmakingStar", 7, 7);
            spriteMap3.AddAnimation("flicker", 0.03f, true, 0, 1, 2, 1);
            spriteMap3.SetAnimation("flicker");
            spriteMap3.CenterOrigin();
            _matchmakingStars.Add(spriteMap3);
            SpriteMap spriteMap4 = new SpriteMap("online/matchmakingStar", 7, 7);
            spriteMap4.AddAnimation("flicker", 0.03f, true, 0, 1, 2, 1);
            spriteMap4.SetAnimation("flicker");
            spriteMap4.CenterOrigin();
            _matchmakingStars.Add(spriteMap4);
        }

        public void ChangeState(MatchmakingState s, float wait = 0f)
        {
            _connectTimeout = 0;
            DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|CHANGE STATE " + s.ToString(), Color.White);
            if (s == MatchmakingState.Waiting)
                return;
            if (wait == 0.0)
            {
                OnStateChange(s);
            }
            else
            {
                _core._state = MatchmakingState.Waiting;
                _pendingState = s;
                _stateWait = wait;
            }
        }

        public bool IsBlacklisted(ulong lobby) => _permenantBlacklist.FirstOrDefault(x => (long)x.lobby == (long)lobby) != null || _failedAttempts.FirstOrDefault(x => (long)x.lobby == (long)lobby) != null || core.blacklist.Contains(lobby);

        private void OnStateChange(MatchmakingState s)
        {
            _core._state = s;
            _stateWait = 0f;
            if (_core._state == MatchmakingState.Disconnect)
            {
                if (!Network.isActive)
                    OnSessionEnded(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Matchmaking disconnect."));
                else
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Matchmaking disconnect."));
            }
            else if (_core._state == MatchmakingState.Searching)
            {
                Network.activeNetwork.core.SearchForLobby();
                DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Searching for lobbies.", Color.White);
            }
            else
            {
                if (_core._state != MatchmakingState.Connecting)
                    return;
                _tryConnectTimeout = 9f + Rando.Float(2f);
                DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Attempting connection to server.", Color.White);
            }
        }

        public override void Open()
        {
            searchTryIndex = 0;
            _permenantBlacklist.Clear();
            _newStatusList.Add("|DGYELLOW|Connecting to servers on the Moon.");
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@ABORT");
            if (playMusic)
                Music.Play("jazzroom");
            _triesSinceSearch = 0;
            triedHostingAlready = false;
            _tryConnectLobby = null;
            _tryHostingLobby = null;
            ChangeState(MatchmakingState.ConnectToMoon);
            _tryHostingWait = 0f;
            _tryConnectTimeout = 0f;
            _quit = false;
            _tries = 0;
            _tryHostingWait = 0f;
            _totalLobbiesFound = -1;
            _failedAttempts.Clear();
            _currentLevel = Level.current;
            _searchingIsOver = false;
            _teamProfileLinks.Clear();
            foreach (Profile key in Profiles.active)
                _teamProfileLinks[key] = key.team;
            base.Open();
        }

        public override void Close()
        {
            ChangeState(MatchmakingState.None);
            _tryHostingWait = 0f;
            if (_quit)
            {
                foreach (KeyValuePair<Profile, Team> teamProfileLink in _teamProfileLinks)
                    teamProfileLink.Key.team = teamProfileLink.Value;
            }
            _quit = false;
            _newStatusList.Clear();
            _statusList.Clear();
            _tryConnectTimeout = 0f;
            base.Close();
        }

        public void OnDisconnect(NetworkConnection n)
        {
            if (!open || _core._state != MatchmakingState.Connecting || _tryHostingLobby == null || Network.connections.Count != 0)
                return;
            ChangeState(MatchmakingState.SearchForLobbies);
            DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Client disconnect, continuing search.", Color.White);
        }

        public void FinishAndClose()
        {
            if (!Network.isActive)
            {
                Level.current = new TeamSelect2();
                Level.UpdateLevelChange();
            }
            HUD.CloseAllCorners();
            Close();
            _openOnClose.Open();
            if (_openOnClose is UIServerBrowser)
                _searchingIsOver = true;
            if (_searchingIsOver)
                MonoMain.pauseMenu = _openOnClose;
            if (Network.isActive || !(Level.current is TeamSelect2) || (Level.current as TeamSelect2)._beam == null)
                return;
            (Level.current as TeamSelect2)._beam.ClearBeam();
        }

        public void OnConnectionError(DuckNetErrorInfo error)
        {
            if (error != null)
            {
                if (error.error == DuckNetError.YourVersionTooNew || error.error == DuckNetError.YourVersionTooOld)
                {
                    if (error.error == DuckNetError.YourVersionTooNew)
                        _newStatusList.Add("|DGRED|Their version was older.");
                    else
                        _newStatusList.Add("|DGRED|Their version was newer.");
                    if (_tryConnectLobby != null)
                        _permenantBlacklist.Add(new BlacklistServer()
                        {
                            lobby = _tryConnectLobby.id,
                            cooldown = 15f
                        });
                }
                else if (error.error == DuckNetError.FullServer)
                    _newStatusList.Add("|DGRED|Failed (FULL SERVER)");
                else if (error.error == DuckNetError.ConnectionTimeout)
                    _newStatusList.Add("|DGRED|Failed (TIMEOUT)");
                else if (error.error == DuckNetError.GameInProgress)
                    _newStatusList.Add("|DGRED|Failed (IN PROGRESS)");
                else if (error.error == DuckNetError.GameNotFoundOrClosed)
                    _newStatusList.Add("|DGRED|Failed (NO LONGER AVAILABLE)");
                else if (error.error == DuckNetError.ClientDisconnected)
                    _newStatusList.Add("|DGYELLOW|Disconnected");
                else if (error.error == DuckNetError.InvalidPassword)
                {
                    _newStatusList.Add("|DGRED|Password was incorrect!");
                }
                else
                {
                    _newStatusList.Add("|DGRED|Unknown connection error.");
                    if (_tryConnectLobby != null)
                        _permenantBlacklist.Add(new BlacklistServer()
                        {
                            lobby = _tryConnectLobby.id,
                            cooldown = 15f
                        });
                }
            }
            else
            {
                _newStatusList.Add("|DGRED|Connection timeout.");
                if (_tryConnectLobby != null)
                    _permenantBlacklist.Add(new BlacklistServer()
                    {
                        lobby = _tryConnectLobby.id,
                        cooldown = 15f
                    });
            }
            if (_tryConnectLobby != null)
                _failedAttempts.Add(new BlacklistServer()
                {
                    lobby = _tryConnectLobby.id,
                    cooldown = 15f
                });
            DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Connection failure, continuing search.", Color.White);
            _tryConnectLobby = null;
            if (_continueSearchOnFail)
            {
                ChangeState(MatchmakingState.SearchForLobbies);
            }
            else
            {
                _searchingIsOver = true;
                _newStatusList.Add("|DGRED|Unable to connect to server.");
                HUD.CloseAllCorners();
                HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@RETURN");
            }
        }

        public void OnSessionEnded(DuckNetErrorInfo error)
        {
            if (!open)
                return;
            if (_core._state == MatchmakingState.Disconnect)
            {
                if (_tryHostingLobby != null)
                    _tries = 0;
                _tryHostingLobby = null;
                if (_quit)
                    FinishAndClose();
                else if (_tryConnectLobby != null)
                {
                    DuckNetwork.Join(_tryConnectLobby.id.ToString());
                    ChangeState(MatchmakingState.Connecting);
                }
                else
                    ChangeState(MatchmakingState.SearchForLobbies);
            }
            else
                OnConnectionError(error);
        }

        public override void Update()
        {
            if (!_searchingIsOver)
            {
                _scroll += 0.1f;
                if (_scroll > 9.0)
                    _scroll = 0f;
                _dots += 0.01f;
                if (_dots > 1.0)
                    _dots = 0f;
            }
            if (open)
            {
                foreach (BlacklistServer failedAttempt in _failedAttempts)
                    failedAttempt.cooldown = Lerp.Float(failedAttempt.cooldown, 0f, Maths.IncFrameTimer());
                if (_searchingIsOver)
                {
                    _signalCrossLocal.SetAnimation("idle");
                    _core.pulseLocal = false;
                }
                else if (_signalCrossLocal.currentAnimation == "idle")
                {
                    if (_core.pulseLocal)
                    {
                        _signalCrossLocal.SetAnimation("flicker");
                        _core.pulseLocal = false;
                    }
                }
                else if (_signalCrossLocal.finished)
                    _signalCrossLocal.SetAnimation("idle");
                if (_signalCrossNetwork.currentAnimation == "idle")
                {
                    if (_core.pulseNetwork)
                    {
                        _signalCrossNetwork.SetAnimation("flicker");
                        _core.pulseNetwork = false;
                    }
                }
                else if (_signalCrossNetwork.finished)
                    _signalCrossNetwork.SetAnimation("idle");
                if (Network.connections.Count > 0 && _core._state != MatchmakingState.Connecting)
                {
                    ChangeState(MatchmakingState.Connecting);
                    DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Network appears to be connecting...", Color.White);
                }
                if (DuckNetwork.status == DuckNetStatus.Connected)
                {
                    if (_tryHostingLobby != null)
                    {
                        (Level.current as TeamSelect2).CloseAllDialogs();
                        Level.current = new TeamSelect2();
                        DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Finished! (HOST).", Color.White);
                        return;
                    }
                    if (Level.current != _currentLevel)
                        return;
                    (Level.current as TeamSelect2).CloseAllDialogs();
                    Level.current = new ConnectingScreen();
                    DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Finished! (CLIENT).", Color.White);
                    return;
                }
                if (_core._state == MatchmakingState.Waiting)
                {
                    _stateWait -= Maths.IncFrameTimer();
                    if (_stateWait <= 0.0)
                    {
                        _stateWait = 0f;
                        OnStateChange(_pendingState);
                    }
                }
                else if (_core._state == MatchmakingState.ConnectToMoon)
                {
                    Network.activeNetwork.core.AddLobbyStringFilter("started", "true", LobbyFilterComparison.Equal);
                    Network.activeNetwork.core.SearchForLobby();
                    Network.activeNetwork.core.RequestGlobalStats();
                    _core.pulseLocal = true;
                    ChangeState(MatchmakingState.ConnectingToMoon);
                }
                else if (_core._state == MatchmakingState.ConnectingToMoon)
                {
                    if (Network.activeNetwork.core.IsLobbySearchComplete())
                    {
                        if (searchTryIndex == 0)
                        {
                            _totalInGameLobbies = Network.activeNetwork.core.NumLobbiesFound();
                            if (_totalInGameLobbies < 0)
                                _totalInGameLobbies = 0;
                            ++searchTryIndex;
                            Network.activeNetwork.core.AddLobbyStringFilter("started", "false", LobbyFilterComparison.Equal);
                            Network.activeNetwork.core.SearchForLobby();
                        }
                        else
                        {
                            _core.pulseNetwork = true;
                            _totalLobbiesFound = Network.activeNetwork.core.NumLobbiesFound();
                            _newStatusList.Add("|DGGREEN|Connected to Moon!");
                            _newStatusList.Add("");
                            _newStatusList.Add("|DGYELLOW|Searching for companions.");
                            ChangeState(MatchmakingState.SearchForLobbies);
                        }
                    }
                }
                else if (_core._state == MatchmakingState.CheckingTotalGames)
                {
                    if (Network.activeNetwork.core.IsLobbySearchComplete())
                    {
                        _totalInGameLobbies = Network.activeNetwork.core.NumLobbiesFound();
                        if (_totalInGameLobbies < 0)
                            _totalInGameLobbies = 0;
                        ChangeState(MatchmakingState.SearchForLobbies);
                        _triesSinceSearch = 0;
                    }
                }
                else if (_core._state == MatchmakingState.SearchForLobbies)
                {
                    if (_triesSinceSearch == 3)
                    {
                        Network.activeNetwork.core.AddLobbyStringFilter("started", "true", LobbyFilterComparison.Equal);
                        Network.activeNetwork.core.SearchForLobby();
                        ChangeState(MatchmakingState.CheckingTotalGames);
                        return;
                    }
                    if (_tries > 0 && _tryHostingLobby == null)
                    {
                        DuckNetwork.Host(TeamSelect2.GetSettingInt("maxplayers"), NetworkLobbyType.Public);
                        _tryHostingLobby = Network.activeNetwork.core.lobby;
                        if (!triedHostingAlready)
                            _newStatusList.Add("|DGYELLOW|Searching even harder.");
                        else
                            _newStatusList.Add("|DGYELLOW|Searching.");
                        triedHostingAlready = true;
                        DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Opened lobby while searching.", Color.White);
                        _tryHostingWait = 5f + Rando.Float(2f);
                    }
                    Network.activeNetwork.core.ApplyTS2LobbyFilters();
                    Network.activeNetwork.core.AddLobbyStringFilter("started", "false", LobbyFilterComparison.Equal);
                    Network.activeNetwork.core.AddLobbyStringFilter("beta", "2.0", LobbyFilterComparison.Equal);
                    Network.activeNetwork.core.AddLobbyStringFilter("dev", DG.devBuild ? "true" : "false", LobbyFilterComparison.Equal);
                    Network.activeNetwork.core.AddLobbyStringFilter("modhash", ModLoader.modHash, LobbyFilterComparison.Equal);
                    Network.activeNetwork.core.AddLobbyStringFilter("password", "false", LobbyFilterComparison.Equal);
                    Network.activeNetwork.core.AddLobbyStringFilter("dedicated", "false", LobbyFilterComparison.Equal);
                    long numKills;
                    if (Network.activeNetwork.core.TryRequestDailyKills(out numKills))
                        _globalKills = numKills;
                    _core.pulseLocal = true;
                    ChangeState(MatchmakingState.Searching);
                    ++_triesSinceSearch;
                    ++_tries;
                }
                else if (_core._state == MatchmakingState.Searching)
                {
                    if (Network.activeNetwork.core.IsLobbySearchComplete())
                    {
                        _totalLobbiesFound = Network.activeNetwork.core.NumLobbiesFound();
                        List<Lobby> lobbyList = new List<Lobby>();
                        DevConsole.Log("|PURPLE|LOBBY    |LIME|found " + Math.Max(_totalLobbiesFound, 0).ToString() + " lobbies.", Color.White);
                    label_91:
                        for (int index1 = 0; index1 < 2; ++index1)
                        {
                            int num1 = index1 != 0 ? lobbyList.Count : Network.activeNetwork.core.NumLobbiesFound();
                            for (int index2 = 0; index2 < num1; ++index2)
                            {
                                Lobby lobby = index1 != 0 ? lobbyList[index2] : Network.activeNetwork.core.GetSearchLobbyAtIndex(index2);
                                if (_tryHostingLobby == null || (long)lobby.id != (long)_tryHostingLobby.id)
                                {
                                    if (index2 == Network.activeNetwork.core.NumLobbiesFound() - 1)
                                        _failedAttempts.RemoveAll(x => x.cooldown <= 0.0);
                                    if (IsBlacklisted(lobby.id))
                                        DevConsole.Log("|PURPLE|LOBBY    |DGRED|Skipping " + lobby.id.ToString() + " (BLACKLISTED)", Color.White);
                                    else if (_core.nonPreferredServers.Contains(lobby.id) && index1 == 0)
                                    {
                                        lobbyList.Add(lobby);
                                        DevConsole.Log("|PURPLE|LOBBY    |DGRED|Skipping " + lobby.id.ToString() + " (NOT PREFERRED)", Color.White);
                                    }
                                    else
                                    {
                                        switch (DuckNetwork.CheckVersion(lobby.GetLobbyData("version")))
                                        {
                                            case NMVersionMismatch.Type.Match:
                                                if (_tryHostingLobby != null)
                                                {
                                                    int num2 = -1;
                                                    try
                                                    {
                                                        string lobbyData = lobby.GetLobbyData("randomID");
                                                        if (lobbyData != "")
                                                            num2 = Convert.ToInt32(lobbyData);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                    if (num2 == -1)
                                                    {
                                                        DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Bad lobby seed.", Color.White);
                                                        num2 = Rando.Int(2147483646);
                                                    }
                                                    if (num2 >= _tryHostingLobby.randomID)
                                                    {
                                                        DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Lobby beats own lobby, Attempting join.", Color.White);
                                                    }
                                                    else
                                                    {
                                                        DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Skipping lobby (Chose to keep hosting).", Color.White);
                                                        Network.activeNetwork.core.UpdateRandomID(_tryHostingLobby);
                                                        continue;
                                                    }
                                                }
                                                _tryConnectLobby = lobby;
                                                if (lobby.owner != null)
                                                    _newStatusList.Add("|LIME|Trying to join " + lobby.owner.name + ".");
                                                else
                                                    _newStatusList.Add("|LIME|Trying to join server.");
                                                ChangeState(MatchmakingState.Disconnect);
                                                goto label_91;
                                            case NMVersionMismatch.Type.Older:
                                                _newStatusList.Add("|PURPLE|LOBBY |DGRED|Skipped(TOO OLD)");
                                                continue;
                                            case NMVersionMismatch.Type.Newer:
                                                _newStatusList.Add("|PURPLE|LOBBY |DGRED|Skipped(TOO NEW)");
                                                continue;
                                            default:
                                                _newStatusList.Add("|PURPLE|LOBBY |DGRED|Skipped(ERROR)");
                                                continue;
                                        }
                                    }
                                }
                            }
                        }
                        if (_tryConnectLobby == null)
                        {
                            DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Found no valid lobbies.", Color.White);
                            ChangeState(MatchmakingState.SearchForLobbies, 3f);
                        }
                    }
                }
                else if (_core._state == MatchmakingState.Connecting)
                {
                    ++_connectTimeout;
                    if (!Network.connected && _connectTimeout > 120)
                    {
                        _tryConnectLobby = null;
                        DevConsole.Log("|PURPLE|LOBBY    |DGRED|Failed to connect!", Color.White);
                        if (this is UIGameConnectionBox)
                        {
                            ChangeState(MatchmakingState.None);
                            _searchingIsOver = true;
                            _newStatusList.Add("|DGRED|Unable to connect to server.");
                            HUD.CloseAllCorners();
                            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@RETURN");
                        }
                        else
                        {
                            _newStatusList.Add("|DGRED|Failed to connect to server!");
                            _newStatusList.Add("|DGORANGE|Back to searching");
                            ChangeState(MatchmakingState.SearchForLobbies, 3f);
                        }
                    }
                }
                UpdateAdditionalMatchmakingLogic();
                if (Input.Pressed(Triggers.Cancel))
                {
                    _quit = true;
                    ChangeState(MatchmakingState.Disconnect);
                }
            }
            if (_newStatusList.Count > 0)
            {
                _newStatusWait -= 0.1f;
                if (_newStatusWait <= 0.0)
                {
                    _newStatusWait = 1f;
                    while (_fancyFont.GetWidth(_newStatusList[0]) > 100.0)
                        _newStatusList[0] = _newStatusList[0].Substring(0, _newStatusList[0].Length - 1);
                    _statusList.Add(_newStatusList[0]);
                    if (_statusList.Count > 7)
                        _statusList.RemoveAt(0);
                    _newStatusList.RemoveAt(0);
                }
            }
            base.Update();
        }

        protected virtual void UpdateAdditionalMatchmakingLogic()
        {
        }

        public void HandleFullGameError(DuckNetError error, string message) => Network.EndNetworkingSession(new DuckNetErrorInfo()
        {
            error = error,
            message = message
        });

        public override void Draw()
        {
            _frame.depth = depth;
            Graphics.Draw(ref _frame, x, y);
            if (!_searchingIsOver)
            {
                for (int index = 0; index < 7; ++index)
                {
                    float num1 = this.x - 28f;
                    float x = num1 + index * 9 + (float)Math.Round(_scroll);
                    float num2 = num1 + 63f;
                    double num3 = (x - num1) / (num2 - num1);
                    _matchmakingSignal.depth = depth + 4;
                    if (num3 > -0.1f)
                        _matchmakingSignal.frame = 0;
                    if (num3 > 0.05f)
                        _matchmakingSignal.frame = 1;
                    if (num3 > 0.1f)
                        _matchmakingSignal.frame = 2;
                    if (num3 > 0.9f)
                        _matchmakingSignal.frame = 1;
                    if (num3 > 0.95f)
                        _matchmakingSignal.frame = 0;
                    Graphics.Draw(ref _matchmakingSignal, x, y - 21f);
                }
            }
            _matchmakingStars[0].depth = depth + 2;
            SpriteMap g1 = _matchmakingStars[0];
            Graphics.Draw(ref g1, x - 9f, y - 18f);
            _matchmakingStars[1].depth = depth + 2;
            SpriteMap g2 = _matchmakingStars[1];
            Graphics.Draw(ref g2, x + 31f, y - 22f);
            _matchmakingStars[2].depth = depth + 2;
            SpriteMap g3 = _matchmakingStars[2];
            Graphics.Draw(ref g3, x + 12f, y - 20f);
            _matchmakingStars[3].depth = depth + 2;
            SpriteMap g4 = _matchmakingStars[3];
            Graphics.Draw(ref g4, x - 23f, y - 21f);
            _signalCrossLocal.depth = depth + 2;
            Graphics.Draw(ref _signalCrossLocal, x - 35f, y - 19f);
            _signalCrossNetwork.depth = depth + 2;
            Graphics.Draw(ref _signalCrossNetwork, x + 45f, y - 23f);
            _font.DrawOutline(_caption, position + new Vec2((float)-(_font.GetWidth(_caption) / 2.0), -42f), Color.White, Color.Black, depth + 2);
            _fancyFont.scale = new Vec2(0.5f);
            int num4 = 0;
            int num5 = 0;
            foreach (string status in _statusList)
            {
                string str1 = status;
                if (num5 == _statusList.Count - 1 && _newStatusList.Count == 0)
                {
                    string str2 = "";
                    if (!_searchingIsOver)
                    {
                        string str3 = ".";
                        if (str1.Length > 0 && str1.Last() == '!' || str1.Last() == '.' || str1.Last() == '?')
                        {
                            str3 = str1.Last().ToString() ?? "";
                            str1 = str1.Substring(0, str1.Length - 1);
                        }
                        for (int index = 0; index < 3; ++index)
                        {
                            if (_dots * 4.0 > index + 1)
                                str2 += str3;
                        }
                        str1 += str2;
                    }
                }
                _fancyFont.Draw(str1, new Vec2(x - 52f, y - 8f + num4 * 6), Color.White, depth + 2);
                ++num4;
                ++num5;
            }
            if (_totalLobbiesFound != -1)
            {
                string str = "games";
                if (_totalLobbiesFound == 1)
                    str = "game";
                if (_totalInGameLobbies > 0)
                    _fancyFont.Draw(_totalLobbiesFound.ToString() + " open " + str + " |DGYELLOW|(" + _totalInGameLobbies.ToString() + " in progress)", position + new Vec2(-55f, 38f), Color.Black, depth + 2);
                else
                    _fancyFont.Draw(_totalLobbiesFound.ToString() + " open " + str, position + new Vec2(-55f, 38f), Color.Black, depth + 2);
            }
            else if (_searchingIsOver)
                _fancyFont.Draw("Could not connect.", position + new Vec2(-55f, 38f), Color.Black, depth + 2);
            else
                _fancyFont.Draw("Querying moon...", position + new Vec2(-55f, 38f), Color.Black, depth + 2);
        }
    }
}
