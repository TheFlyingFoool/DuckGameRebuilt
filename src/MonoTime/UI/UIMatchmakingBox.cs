// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMatchmakingBox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            get => UIMatchmakingBox._core;
            set => UIMatchmakingBox._core = value;
        }

        public List<MatchmakingPlayer> matchmakingProfiles
        {
            get => UIMatchmakingBox.core.matchmakingProfiles;
            set => UIMatchmakingBox.core.matchmakingProfiles = value;
        }

        public UIMatchmakingBox(UIMenu openOnClose, float xpos, float ypos, float wide = -1f, float high = -1f)
          : base("", xpos, ypos, wide, high)
        {
            this._openOnClose = openOnClose;
            Graphics.fade = 1f;
            this._frame = new Sprite("online/matchmaking");
            this._frame.CenterOrigin();
            this._font = new BitmapFont("biosFontUI", 8, 7);
            this._fancyFont = new FancyBitmapFont("smallFont");
            this._matchmakingSignal = new SpriteMap("online/matchmakingSignal", 4, 9);
            this._matchmakingSignal.CenterOrigin();
            SpriteMap spriteMap1 = new SpriteMap("online/matchmakingStar", 7, 7);
            spriteMap1.AddAnimation("flicker", 0.08f, true, 0, 1, 2, 1);
            spriteMap1.SetAnimation("flicker");
            spriteMap1.CenterOrigin();
            this._signalCrossLocal = new SpriteMap("online/signalCross", 5, 5);
            this._signalCrossLocal.AddAnimation("idle", 0.12f, true, new int[1]);
            this._signalCrossLocal.AddAnimation("flicker", 0.12f, false, 1, 2, 3);
            this._signalCrossLocal.SetAnimation("idle");
            this._signalCrossLocal.CenterOrigin();
            this._signalCrossNetwork = new SpriteMap("online/signalCross", 5, 5);
            this._signalCrossNetwork.AddAnimation("idle", 0.12f, true, new int[1]);
            this._signalCrossNetwork.AddAnimation("flicker", 0.12f, false, 1, 2, 3);
            this._signalCrossNetwork.SetAnimation("idle");
            this._signalCrossNetwork.CenterOrigin();
            this._matchmakingStars.Add(spriteMap1);
            SpriteMap spriteMap2 = new SpriteMap("online/matchmakingStar", 7, 7);
            spriteMap2.AddAnimation("flicker", 0.11f, true, 0, 1, 2, 1);
            spriteMap2.SetAnimation("flicker");
            spriteMap2.CenterOrigin();
            this._matchmakingStars.Add(spriteMap2);
            SpriteMap spriteMap3 = new SpriteMap("online/matchmakingStar", 7, 7);
            spriteMap3.AddAnimation("flicker", 0.03f, true, 0, 1, 2, 1);
            spriteMap3.SetAnimation("flicker");
            spriteMap3.CenterOrigin();
            this._matchmakingStars.Add(spriteMap3);
            SpriteMap spriteMap4 = new SpriteMap("online/matchmakingStar", 7, 7);
            spriteMap4.AddAnimation("flicker", 0.03f, true, 0, 1, 2, 1);
            spriteMap4.SetAnimation("flicker");
            spriteMap4.CenterOrigin();
            this._matchmakingStars.Add(spriteMap4);
        }

        public void ChangeState(MatchmakingState s, float wait = 0.0f)
        {
            this._connectTimeout = 0;
            DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|CHANGE STATE " + s.ToString(), Color.White);
            if (s == MatchmakingState.Waiting)
                return;
            if ((double)wait == 0.0)
            {
                this.OnStateChange(s);
            }
            else
            {
                UIMatchmakingBox._core._state = MatchmakingState.Waiting;
                this._pendingState = s;
                this._stateWait = wait;
            }
        }

        public bool IsBlacklisted(ulong lobby) => this._permenantBlacklist.FirstOrDefault<BlacklistServer>((Func<BlacklistServer, bool>)(x => (long)x.lobby == (long)lobby)) != null || this._failedAttempts.FirstOrDefault<BlacklistServer>((Func<BlacklistServer, bool>)(x => (long)x.lobby == (long)lobby)) != null || UIMatchmakingBox.core.blacklist.Contains(lobby);

        private void OnStateChange(MatchmakingState s)
        {
            UIMatchmakingBox._core._state = s;
            this._stateWait = 0.0f;
            if (UIMatchmakingBox._core._state == MatchmakingState.Disconnect)
            {
                if (!Network.isActive)
                    this.OnSessionEnded(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Matchmaking disconnect."));
                else
                    Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Matchmaking disconnect."));
            }
            else if (UIMatchmakingBox._core._state == MatchmakingState.Searching)
            {
                Network.activeNetwork.core.SearchForLobby();
                DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Searching for lobbies.", Color.White);
            }
            else
            {
                if (UIMatchmakingBox._core._state != MatchmakingState.Connecting)
                    return;
                this._tryConnectTimeout = 9f + Rando.Float(2f);
                DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Attempting connection to server.", Color.White);
            }
        }

        public override void Open()
        {
            this.searchTryIndex = 0;
            this._permenantBlacklist.Clear();
            this._newStatusList.Add("|DGYELLOW|Connecting to servers on the Moon.");
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@ABORT");
            if (this.playMusic)
                Music.Play("jazzroom");
            this._triesSinceSearch = 0;
            this.triedHostingAlready = false;
            this._tryConnectLobby = (Lobby)null;
            this._tryHostingLobby = (Lobby)null;
            this.ChangeState(MatchmakingState.ConnectToMoon);
            this._tryHostingWait = 0.0f;
            this._tryConnectTimeout = 0.0f;
            this._quit = false;
            this._tries = 0;
            this._tryHostingWait = 0.0f;
            this._totalLobbiesFound = -1;
            this._failedAttempts.Clear();
            this._currentLevel = Level.current;
            this._searchingIsOver = false;
            this._teamProfileLinks.Clear();
            foreach (Profile key in Profiles.active)
                this._teamProfileLinks[key] = key.team;
            base.Open();
        }

        public override void Close()
        {
            this.ChangeState(MatchmakingState.None);
            this._tryHostingWait = 0.0f;
            if (this._quit)
            {
                foreach (KeyValuePair<Profile, Team> teamProfileLink in this._teamProfileLinks)
                    teamProfileLink.Key.team = teamProfileLink.Value;
            }
            this._quit = false;
            this._newStatusList.Clear();
            this._statusList.Clear();
            this._tryConnectTimeout = 0.0f;
            base.Close();
        }

        public void OnDisconnect(NetworkConnection n)
        {
            if (!this.open || UIMatchmakingBox._core._state != MatchmakingState.Connecting || this._tryHostingLobby == null || Network.connections.Count != 0)
                return;
            this.ChangeState(MatchmakingState.SearchForLobbies);
            DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Client disconnect, continuing search.", Color.White);
        }

        public void FinishAndClose()
        {
            if (!Network.isActive)
            {
                Level.current = (Level)new TeamSelect2();
                Level.UpdateLevelChange();
            }
            HUD.CloseAllCorners();
            this.Close();
            this._openOnClose.Open();
            if (this._openOnClose is UIServerBrowser)
                this._searchingIsOver = true;
            if (this._searchingIsOver)
                MonoMain.pauseMenu = (UIComponent)this._openOnClose;
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
                        this._newStatusList.Add("|DGRED|Their version was older.");
                    else
                        this._newStatusList.Add("|DGRED|Their version was newer.");
                    if (this._tryConnectLobby != null)
                        this._permenantBlacklist.Add(new BlacklistServer()
                        {
                            lobby = this._tryConnectLobby.id,
                            cooldown = 15f
                        });
                }
                else if (error.error == DuckNetError.FullServer)
                    this._newStatusList.Add("|DGRED|Failed (FULL SERVER)");
                else if (error.error == DuckNetError.ConnectionTimeout)
                    this._newStatusList.Add("|DGRED|Failed (TIMEOUT)");
                else if (error.error == DuckNetError.GameInProgress)
                    this._newStatusList.Add("|DGRED|Failed (IN PROGRESS)");
                else if (error.error == DuckNetError.GameNotFoundOrClosed)
                    this._newStatusList.Add("|DGRED|Failed (NO LONGER AVAILABLE)");
                else if (error.error == DuckNetError.ClientDisconnected)
                    this._newStatusList.Add("|DGYELLOW|Disconnected");
                else if (error.error == DuckNetError.InvalidPassword)
                {
                    this._newStatusList.Add("|DGRED|Password was incorrect!");
                }
                else
                {
                    this._newStatusList.Add("|DGRED|Unknown connection error.");
                    if (this._tryConnectLobby != null)
                        this._permenantBlacklist.Add(new BlacklistServer()
                        {
                            lobby = this._tryConnectLobby.id,
                            cooldown = 15f
                        });
                }
            }
            else
            {
                this._newStatusList.Add("|DGRED|Connection timeout.");
                if (this._tryConnectLobby != null)
                    this._permenantBlacklist.Add(new BlacklistServer()
                    {
                        lobby = this._tryConnectLobby.id,
                        cooldown = 15f
                    });
            }
            if (this._tryConnectLobby != null)
                this._failedAttempts.Add(new BlacklistServer()
                {
                    lobby = this._tryConnectLobby.id,
                    cooldown = 15f
                });
            DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Connection failure, continuing search.", Color.White);
            this._tryConnectLobby = (Lobby)null;
            if (this._continueSearchOnFail)
            {
                this.ChangeState(MatchmakingState.SearchForLobbies);
            }
            else
            {
                this._searchingIsOver = true;
                this._newStatusList.Add("|DGRED|Unable to connect to server.");
                HUD.CloseAllCorners();
                HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@RETURN");
            }
        }

        public void OnSessionEnded(DuckNetErrorInfo error)
        {
            if (!this.open)
                return;
            if (UIMatchmakingBox._core._state == MatchmakingState.Disconnect)
            {
                if (this._tryHostingLobby != null)
                    this._tries = 0;
                this._tryHostingLobby = (Lobby)null;
                if (this._quit)
                    this.FinishAndClose();
                else if (this._tryConnectLobby != null)
                {
                    DuckNetwork.Join(this._tryConnectLobby.id.ToString());
                    this.ChangeState(MatchmakingState.Connecting);
                }
                else
                    this.ChangeState(MatchmakingState.SearchForLobbies);
            }
            else
                this.OnConnectionError(error);
        }

        public override void Update()
        {
            if (!this._searchingIsOver)
            {
                this._scroll += 0.1f;
                if ((double)this._scroll > 9.0)
                    this._scroll = 0.0f;
                this._dots += 0.01f;
                if ((double)this._dots > 1.0)
                    this._dots = 0.0f;
            }
            if (this.open)
            {
                foreach (BlacklistServer failedAttempt in this._failedAttempts)
                    failedAttempt.cooldown = Lerp.Float(failedAttempt.cooldown, 0.0f, Maths.IncFrameTimer());
                if (this._searchingIsOver)
                {
                    this._signalCrossLocal.SetAnimation("idle");
                    UIMatchmakingBox._core.pulseLocal = false;
                }
                else if (this._signalCrossLocal.currentAnimation == "idle")
                {
                    if (UIMatchmakingBox._core.pulseLocal)
                    {
                        this._signalCrossLocal.SetAnimation("flicker");
                        UIMatchmakingBox._core.pulseLocal = false;
                    }
                }
                else if (this._signalCrossLocal.finished)
                    this._signalCrossLocal.SetAnimation("idle");
                if (this._signalCrossNetwork.currentAnimation == "idle")
                {
                    if (UIMatchmakingBox._core.pulseNetwork)
                    {
                        this._signalCrossNetwork.SetAnimation("flicker");
                        UIMatchmakingBox._core.pulseNetwork = false;
                    }
                }
                else if (this._signalCrossNetwork.finished)
                    this._signalCrossNetwork.SetAnimation("idle");
                if (Network.connections.Count > 0 && UIMatchmakingBox._core._state != MatchmakingState.Connecting)
                {
                    this.ChangeState(MatchmakingState.Connecting);
                    DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Network appears to be connecting...", Color.White);
                }
                if (DuckNetwork.status == DuckNetStatus.Connected)
                {
                    if (this._tryHostingLobby != null)
                    {
                        (Level.current as TeamSelect2).CloseAllDialogs();
                        Level.current = (Level)new TeamSelect2();
                        DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Finished! (HOST).", Color.White);
                        return;
                    }
                    if (Level.current != this._currentLevel)
                        return;
                    (Level.current as TeamSelect2).CloseAllDialogs();
                    Level.current = (Level)new ConnectingScreen();
                    DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Finished! (CLIENT).", Color.White);
                    return;
                }
                if (UIMatchmakingBox._core._state == MatchmakingState.Waiting)
                {
                    this._stateWait -= Maths.IncFrameTimer();
                    if ((double)this._stateWait <= 0.0)
                    {
                        this._stateWait = 0.0f;
                        this.OnStateChange(this._pendingState);
                    }
                }
                else if (UIMatchmakingBox._core._state == MatchmakingState.ConnectToMoon)
                {
                    Network.activeNetwork.core.AddLobbyStringFilter("started", "true", LobbyFilterComparison.Equal);
                    Network.activeNetwork.core.SearchForLobby();
                    Network.activeNetwork.core.RequestGlobalStats();
                    UIMatchmakingBox._core.pulseLocal = true;
                    this.ChangeState(MatchmakingState.ConnectingToMoon);
                }
                else if (UIMatchmakingBox._core._state == MatchmakingState.ConnectingToMoon)
                {
                    if (Network.activeNetwork.core.IsLobbySearchComplete())
                    {
                        if (this.searchTryIndex == 0)
                        {
                            this._totalInGameLobbies = Network.activeNetwork.core.NumLobbiesFound();
                            if (this._totalInGameLobbies < 0)
                                this._totalInGameLobbies = 0;
                            ++this.searchTryIndex;
                            Network.activeNetwork.core.AddLobbyStringFilter("started", "false", LobbyFilterComparison.Equal);
                            Network.activeNetwork.core.SearchForLobby();
                        }
                        else
                        {
                            UIMatchmakingBox._core.pulseNetwork = true;
                            this._totalLobbiesFound = Network.activeNetwork.core.NumLobbiesFound();
                            this._newStatusList.Add("|DGGREEN|Connected to Moon!");
                            this._newStatusList.Add("");
                            this._newStatusList.Add("|DGYELLOW|Searching for companions.");
                            this.ChangeState(MatchmakingState.SearchForLobbies);
                        }
                    }
                }
                else if (UIMatchmakingBox._core._state == MatchmakingState.CheckingTotalGames)
                {
                    if (Network.activeNetwork.core.IsLobbySearchComplete())
                    {
                        this._totalInGameLobbies = Network.activeNetwork.core.NumLobbiesFound();
                        if (this._totalInGameLobbies < 0)
                            this._totalInGameLobbies = 0;
                        this.ChangeState(MatchmakingState.SearchForLobbies);
                        this._triesSinceSearch = 0;
                    }
                }
                else if (UIMatchmakingBox._core._state == MatchmakingState.SearchForLobbies)
                {
                    if (this._triesSinceSearch == 3)
                    {
                        Network.activeNetwork.core.AddLobbyStringFilter("started", "true", LobbyFilterComparison.Equal);
                        Network.activeNetwork.core.SearchForLobby();
                        this.ChangeState(MatchmakingState.CheckingTotalGames);
                        return;
                    }
                    if (this._tries > 0 && this._tryHostingLobby == null)
                    {
                        DuckNetwork.Host(TeamSelect2.GetSettingInt("maxplayers"), NetworkLobbyType.Public);
                        this._tryHostingLobby = Network.activeNetwork.core.lobby;
                        if (!this.triedHostingAlready)
                            this._newStatusList.Add("|DGYELLOW|Searching even harder.");
                        else
                            this._newStatusList.Add("|DGYELLOW|Searching.");
                        this.triedHostingAlready = true;
                        DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Opened lobby while searching.", Color.White);
                        this._tryHostingWait = 5f + Rando.Float(2f);
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
                        this._globalKills = numKills;
                    UIMatchmakingBox._core.pulseLocal = true;
                    this.ChangeState(MatchmakingState.Searching);
                    ++this._triesSinceSearch;
                    ++this._tries;
                }
                else if (UIMatchmakingBox._core._state == MatchmakingState.Searching)
                {
                    if (Network.activeNetwork.core.IsLobbySearchComplete())
                    {
                        this._totalLobbiesFound = Network.activeNetwork.core.NumLobbiesFound();
                        List<Lobby> lobbyList = new List<Lobby>();
                        DevConsole.Log("|PURPLE|LOBBY    |LIME|found " + Math.Max(this._totalLobbiesFound, 0).ToString() + " lobbies.", Color.White);
                    label_91:
                        for (int index1 = 0; index1 < 2; ++index1)
                        {
                            int num1 = index1 != 0 ? lobbyList.Count : Network.activeNetwork.core.NumLobbiesFound();
                            for (int index2 = 0; index2 < num1; ++index2)
                            {
                                Lobby lobby = index1 != 0 ? lobbyList[index2] : Network.activeNetwork.core.GetSearchLobbyAtIndex(index2);
                                if (this._tryHostingLobby == null || (long)lobby.id != (long)this._tryHostingLobby.id)
                                {
                                    if (index2 == Network.activeNetwork.core.NumLobbiesFound() - 1)
                                        this._failedAttempts.RemoveAll((Predicate<BlacklistServer>)(x => (double)x.cooldown <= 0.0));
                                    if (this.IsBlacklisted(lobby.id))
                                        DevConsole.Log("|PURPLE|LOBBY    |DGRED|Skipping " + lobby.id.ToString() + " (BLACKLISTED)", Color.White);
                                    else if (UIMatchmakingBox._core.nonPreferredServers.Contains(lobby.id) && index1 == 0)
                                    {
                                        lobbyList.Add(lobby);
                                        DevConsole.Log("|PURPLE|LOBBY    |DGRED|Skipping " + lobby.id.ToString() + " (NOT PREFERRED)", Color.White);
                                    }
                                    else
                                    {
                                        switch (DuckNetwork.CheckVersion(lobby.GetLobbyData("version")))
                                        {
                                            case NMVersionMismatch.Type.Match:
                                                if (this._tryHostingLobby != null)
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
                                                    if (num2 >= this._tryHostingLobby.randomID)
                                                    {
                                                        DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Lobby beats own lobby, Attempting join.", Color.White);
                                                    }
                                                    else
                                                    {
                                                        DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Skipping lobby (Chose to keep hosting).", Color.White);
                                                        Network.activeNetwork.core.UpdateRandomID(this._tryHostingLobby);
                                                        continue;
                                                    }
                                                }
                                                this._tryConnectLobby = lobby;
                                                if (lobby.owner != null)
                                                    this._newStatusList.Add("|LIME|Trying to join " + lobby.owner.name + ".");
                                                else
                                                    this._newStatusList.Add("|LIME|Trying to join server.");
                                                this.ChangeState(MatchmakingState.Disconnect);
                                                goto label_91;
                                            case NMVersionMismatch.Type.Older:
                                                this._newStatusList.Add("|PURPLE|LOBBY |DGRED|Skipped(TOO OLD)");
                                                continue;
                                            case NMVersionMismatch.Type.Newer:
                                                this._newStatusList.Add("|PURPLE|LOBBY |DGRED|Skipped(TOO NEW)");
                                                continue;
                                            default:
                                                this._newStatusList.Add("|PURPLE|LOBBY |DGRED|Skipped(ERROR)");
                                                continue;
                                        }
                                    }
                                }
                            }
                        }
                        if (this._tryConnectLobby == null)
                        {
                            DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Found no valid lobbies.", Color.White);
                            this.ChangeState(MatchmakingState.SearchForLobbies, 3f);
                        }
                    }
                }
                else if (UIMatchmakingBox._core._state == MatchmakingState.Connecting)
                {
                    ++this._connectTimeout;
                    if (!Network.connected && this._connectTimeout > 120)
                    {
                        this._tryConnectLobby = (Lobby)null;
                        DevConsole.Log("|PURPLE|LOBBY    |DGRED|Failed to connect!", Color.White);
                        if (this is UIGameConnectionBox)
                        {
                            this.ChangeState(MatchmakingState.None);
                            this._searchingIsOver = true;
                            this._newStatusList.Add("|DGRED|Unable to connect to server.");
                            HUD.CloseAllCorners();
                            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@RETURN");
                        }
                        else
                        {
                            this._newStatusList.Add("|DGRED|Failed to connect to server!");
                            this._newStatusList.Add("|DGORANGE|Back to searching");
                            this.ChangeState(MatchmakingState.SearchForLobbies, 3f);
                        }
                    }
                }
                this.UpdateAdditionalMatchmakingLogic();
                if (Input.Pressed("CANCEL"))
                {
                    this._quit = true;
                    this.ChangeState(MatchmakingState.Disconnect);
                }
            }
            if (this._newStatusList.Count > 0)
            {
                this._newStatusWait -= 0.1f;
                if ((double)this._newStatusWait <= 0.0)
                {
                    this._newStatusWait = 1f;
                    while ((double)this._fancyFont.GetWidth(this._newStatusList[0]) > 100.0)
                        this._newStatusList[0] = this._newStatusList[0].Substring(0, this._newStatusList[0].Length - 1);
                    this._statusList.Add(this._newStatusList[0]);
                    if (this._statusList.Count > 7)
                        this._statusList.RemoveAt(0);
                    this._newStatusList.RemoveAt(0);
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
            this._frame.depth = this.depth;
            Graphics.Draw(this._frame, this.x, this.y);
            if (!this._searchingIsOver)
            {
                for (int index = 0; index < 7; ++index)
                {
                    float num1 = this.x - 28f;
                    float x = num1 + (float)(index * 9) + (float)Math.Round((double)this._scroll);
                    float num2 = num1 + 63f;
                    double num3 = ((double)x - (double)num1) / ((double)num2 - (double)num1);
                    this._matchmakingSignal.depth = this.depth + 4;
                    if (num3 > -0.100000001490116)
                        this._matchmakingSignal.frame = 0;
                    if (num3 > 0.0500000007450581)
                        this._matchmakingSignal.frame = 1;
                    if (num3 > 0.100000001490116)
                        this._matchmakingSignal.frame = 2;
                    if (num3 > 0.899999976158142)
                        this._matchmakingSignal.frame = 1;
                    if (num3 > 0.949999988079071)
                        this._matchmakingSignal.frame = 0;
                    Graphics.Draw((Sprite)this._matchmakingSignal, x, this.y - 21f);
                }
            }
            this._matchmakingStars[0].depth = this.depth + 2;
            Graphics.Draw((Sprite)this._matchmakingStars[0], this.x - 9f, this.y - 18f);
            this._matchmakingStars[1].depth = this.depth + 2;
            Graphics.Draw((Sprite)this._matchmakingStars[1], this.x + 31f, this.y - 22f);
            this._matchmakingStars[2].depth = this.depth + 2;
            Graphics.Draw((Sprite)this._matchmakingStars[2], this.x + 12f, this.y - 20f);
            this._matchmakingStars[3].depth = this.depth + 2;
            Graphics.Draw((Sprite)this._matchmakingStars[3], this.x - 23f, this.y - 21f);
            this._signalCrossLocal.depth = this.depth + 2;
            Graphics.Draw((Sprite)this._signalCrossLocal, this.x - 35f, this.y - 19f);
            this._signalCrossNetwork.depth = this.depth + 2;
            Graphics.Draw((Sprite)this._signalCrossNetwork, this.x + 45f, this.y - 23f);
            this._font.DrawOutline(this._caption, this.position + new Vec2((float)-((double)this._font.GetWidth(this._caption) / 2.0), -42f), Color.White, Color.Black, this.depth + 2);
            this._fancyFont.scale = new Vec2(0.5f);
            int num4 = 0;
            int num5 = 0;
            foreach (string status in this._statusList)
            {
                string str1 = status;
                if (num5 == this._statusList.Count - 1 && this._newStatusList.Count == 0)
                {
                    string str2 = "";
                    if (!this._searchingIsOver)
                    {
                        string str3 = ".";
                        if (str1.Count<char>() > 0 && str1.Last<char>() == '!' || str1.Last<char>() == '.' || str1.Last<char>() == '?')
                        {
                            str3 = str1.Last<char>().ToString() ?? "";
                            str1 = str1.Substring(0, str1.Length - 1);
                        }
                        for (int index = 0; index < 3; ++index)
                        {
                            if ((double)this._dots * 4.0 > (double)(index + 1))
                                str2 += str3;
                        }
                        str1 += str2;
                    }
                }
                this._fancyFont.Draw(str1, new Vec2(this.x - 52f, this.y - 8f + (float)(num4 * 6)), Color.White, this.depth + 2);
                ++num4;
                ++num5;
            }
            if (this._totalLobbiesFound != -1)
            {
                string str = "games";
                if (this._totalLobbiesFound == 1)
                    str = "game";
                if (this._totalInGameLobbies > 0)
                    this._fancyFont.Draw(this._totalLobbiesFound.ToString() + " open " + str + " |DGYELLOW|(" + this._totalInGameLobbies.ToString() + " in progress)", this.position + new Vec2(-55f, 38f), Color.Black, this.depth + 2);
                else
                    this._fancyFont.Draw(this._totalLobbiesFound.ToString() + " open " + str, this.position + new Vec2(-55f, 38f), Color.Black, this.depth + 2);
            }
            else if (this._searchingIsOver)
                this._fancyFont.Draw("Could not connect.", this.position + new Vec2(-55f, 38f), Color.Black, this.depth + 2);
            else
                this._fancyFont.Draw("Querying moon...", this.position + new Vec2(-55f, 38f), Color.Black, this.depth + 2);
        }
    }
}
