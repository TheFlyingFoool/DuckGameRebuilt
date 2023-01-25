// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMatchmakerMark2
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIMatchmakerMark2 : UIMenu
    {
        private static Core _core = new Core();
        public static bool pulseLocal;
        public static bool pulseNetwork;
        protected bool _continueSearchOnFail = true;
        private UIMenu _openOnClose;
        private Sprite _window;
        private BitmapFont _font;
        private FancyBitmapFont _fancyFont;
        private SpriteMap _signalCrossLocal;
        private SpriteMap _signalCrossNetwork;
        private SpriteMap _matchmakingSignal;
        private List<SpriteMap> _matchmakingStars = new List<SpriteMap>();
        private bool playMusic = true;
        protected int _totalLobbies = -1;
        protected int _joinableLobbies = -1;
        protected State _state;
        protected State _previousState;
        protected int _timeInState;
        protected UIServerBrowser.LobbyData _directConnectLobby;
        private int _timeOpen;
        private static Level _currentLevel;
        public Lobby _hostedLobby;
        public Lobby _processing;
        public HashSet<ulong> blacklist = new HashSet<ulong>();
        public HashSet<ulong> attempted = new HashSet<ulong>();
        protected int _wait;
        public static int searchMode = 1;
        protected bool _resetNetwork;
        private int _framesSinceReset;
        private bool _resetting;
        protected string _passwordAttempt = "";
        protected List<string> messages = new List<string>();
        private float _scroll;
        private float _dots;
        protected string _caption = "MATCHMAKING";

        public static Core core
        {
            get => _core;
            set => _core = value;
        }

        protected void ChangeState(State pState)
        {
            if (_directConnectLobby != null && (pState == State.TryJoiningLobbies || pState == State.SearchForLobbies))
                pState = State.Failed;
            if (pState == State.Failed && _previousState != State.Failed)
            {
                HUD.CloseAllCorners();
                HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@RETURN");
                messages.Add("|DGRED|Unable to connect to server.");
            }
            _previousState = _state;
            _state = pState;
            _timeInState = 0;
            Wait();
        }

        public static UIMatchmakerMark2 Platform_GetMatchkmaker(
          UIServerBrowser.LobbyData joinLobby,
          UIMenu openOnClose)
        {
            return new UIMatchmakerSteam(joinLobby, openOnClose);
        }

        protected UIMatchmakerMark2(UIServerBrowser.LobbyData joinLobby, UIMenu openOnClose)
          : this(openOnClose)
        {
            _directConnectLobby = joinLobby;
        }

        protected UIMatchmakerMark2(UIMenu openOnClose)
          : base("", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f)
        {
            _openOnClose = openOnClose;
            Graphics.fade = 1f;
            _window = new Sprite("online/matchmaking_mk2");
            _window.CenterOrigin();
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

        protected virtual void Platform_Open()
        {
        }

        public override void Open()
        {
            _state = State.InitializeMatchmaking;
            Platform_Open();
            _timeOpen = 0;
            _currentLevel = Level.current;
            instance = this;
            _processing = null;
            messages.Clear();
            if (_directConnectLobby != null)
                _state = State.TryJoiningLobbies;
            else
                messages.Add("|DGYELLOW|Connecting to servers on the Moon...");
            _totalLobbies = -1;
            _joinableLobbies = -1;
            attempted.Clear();
            blacklist.Clear();
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@ABORT");
            if (playMusic && _directConnectLobby == null)
                Music.Play("jazzroom");
            base.Open();
        }

        public override void Close()
        {
            if (instance == this)
                instance = null;
            _state = State.Idle;
            base.Close();
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
            if (Network.isActive)
                return;
            if (_openOnClose is UIServerBrowser)
            {
                _openOnClose.Open();
                MonoMain.pauseMenu = _openOnClose;
            }
            else
                MonoMain.pauseMenu = null;
            if (Network.isActive || !(Level.current is TeamSelect2) || (Level.current as TeamSelect2)._beam == null)
                return;
            (Level.current as TeamSelect2)._beam.ClearBeam();
        }

        public void Hook_OnSessionEnded(DuckNetErrorInfo error)
        {
            _resetting = false;
            _framesSinceReset = 0;
            if (_state != State.Aborting)
            {
                if (_hostedLobby == null)
                {
                    if (error == null || error.error != DuckNetError.HostIsABlockedUser)
                    {
                        messages.Add("|PURPLE|LOBBY    |DGRED|Connection to lobby failed:");
                        OnConnectionError(error);
                        if (_directConnectLobby == null)
                            messages.Add("|PURPLE|LOBBY    |WHITE|Looking for more lobbies to try...");
                    }
                    _wait += 60;
                }
                ChangeState(State.TryJoiningLobbies);
            }
            _processing = null;
            _hostedLobby = null;
        }

        public virtual void Hook_OnLobbyProcessed(object pLobby)
        {
        }

        public void OnConnectionError(DuckNetErrorInfo error)
        {
            if (error != null)
            {
                if (error.error == DuckNetError.YourVersionTooNew || error.error == DuckNetError.YourVersionTooOld)
                {
                    if (error.error == DuckNetError.YourVersionTooNew)
                        messages.Add("|DGRED|Their version was older.");
                    else
                        messages.Add("|DGRED|Their version was newer.");
                    if (_processing == null)
                        return;
                    blacklist.Add(_processing.id);
                }
                else if (error.error == DuckNetError.FullServer)
                    messages.Add("|DGRED|Failed (FULL SERVER)");
                else if (error.error == DuckNetError.ConnectionTimeout)
                    messages.Add("|DGRED|Failed (TIMEOUT)");
                else if (error.error == DuckNetError.GameInProgress)
                    messages.Add("|DGRED|Failed (IN PROGRESS)");
                else if (error.error == DuckNetError.GameNotFoundOrClosed)
                    messages.Add("|DGRED|Failed (NO LONGER AVAILABLE)");
                else if (error.error == DuckNetError.ClientDisconnected)
                    messages.Add("|DGYELLOW|Disconnected");
                else if (error.error == DuckNetError.InvalidPassword)
                    messages.Add("|DGRED|Password was incorrect!");
                else if (error.error == DuckNetError.ModsIncompatible)
                {
                    messages.Add("|DGRED|Host had different mods enabled!");
                }
                else
                {
                    if (error.error == DuckNetError.HostIsABlockedUser)
                        return;
                    messages.Add("|DGRED|Unknown connection error.");
                    if (_processing == null)
                        return;
                    blacklist.Add(_processing.id);
                }
            }
            else
            {
                messages.Add("|DGRED|Connection timeout.");
                if (_processing == null)
                    return;
                blacklist.Add(_processing.id);
            }
        }

        public virtual void Hook_OnDucknetJoined()
        {
            if (Level.current is TeamSelect2)
                (Level.current as TeamSelect2).CloseAllDialogs();
            if (Network.isServer)
            {
                Level.current = new TeamSelect2
                {
                    suppressLevelMessage = true
                };
            }
            Close();
            DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Finished! (HOST).", Color.White);
        }

        private void Wait() => _wait += 60;

        protected bool HostLobby()
        {
            if (_hostedLobby == null && Reset())
            {
                messages.Add("|DGYELLOW|Having trouble finding an open lobby...");
                messages.Add("|DGGREEN|Creating a lobby of our very own...");
                DuckNetwork.Host(TeamSelect2.GetSettingInt("maxplayers"), NetworkLobbyType.Public);
                _hostedLobby = Network.activeNetwork.core.lobby;
                DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Opened lobby while searching.", Color.White);
                _wait = 280 + Rando.Int(120);
            }
            return _hostedLobby != null;
        }

        protected virtual void Platform_ResetLogic()
        {
        }

        protected bool Reset()
        {
            if (Network.isActive)
            {
                Platform_ResetLogic();
                _resetNetwork = true;
                Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Matchmaking disconnect."));
                _resetting = true;
                _framesSinceReset = 0;
            }
            return !Network.isActive;
        }

        public static UIMatchmakerMark2 instance
        {
            get
            {
                if (_currentLevel != Level.current)
                    _core.instance = null;
                return _core.instance;
            }
            set => _core.instance = value;
        }

        public virtual void Platform_Update()
        {
        }

        public virtual void Platform_MatchmakerLogic()
        {
        }

        public override void Update()
        {
            if (!open)
                return;
            ++_timeInState;
            if (instance == null)
                FinishAndClose();
            else if (_resetNetwork && Network.isActive)
            {
                Reset();
            }
            else
            {
                _resetNetwork = false;
                if (Input.Pressed(Triggers.Cancel))
                {
                    Reset();
                    messages.Add("|DGRED|Aborting...");
                    ChangeState(State.Aborting);
                }
                if (_resetting)
                {
                    ++_framesSinceReset;
                    if (_framesSinceReset <= 120)
                        return;
                    Network.Terminate();
                    if (_state != State.Aborting)
                        ChangeState(State.TryJoiningLobbies);
                    _processing = null;
                    _hostedLobby = null;
                }
                else
                {
                    ++_timeOpen;
                    if (_timeOpen > 7200)
                        attempted.Clear();
                    Platform_Update();
                    if (_wait > 0 && _state != State.Aborting && _state != State.JoinLobby)
                        --_wait;
                    else
                        Platform_MatchmakerLogic();
                }
            }
        }

        public void SetPasswordAttempt(string pPassword) => _passwordAttempt = pPassword;

        public override void Draw()
        {
            if (!open)
                return;
            _window.depth = depth;
            Graphics.Draw(_window, x, y);
            _scroll += 0.06f;
            if (_scroll > 9.0)
                _scroll = 0f;
            _dots += 0.01f;
            if (_dots > 1.0)
                _dots = 0f;
            if (_state == State.Idle || _state == State.Failed)
            {
                _signalCrossLocal.SetAnimation("idle");
                pulseLocal = false;
            }
            else if (_signalCrossLocal.currentAnimation == "idle")
            {
                if (pulseLocal)
                {
                    _signalCrossLocal.SetAnimation("flicker");
                    pulseLocal = false;
                }
            }
            else if (_signalCrossLocal.finished)
                _signalCrossLocal.SetAnimation("idle");
            if (_signalCrossNetwork.currentAnimation == "idle")
            {
                if (pulseNetwork)
                {
                    _signalCrossNetwork.SetAnimation("flicker");
                    pulseNetwork = false;
                }
            }
            else if (_signalCrossNetwork.finished)
                _signalCrossNetwork.SetAnimation("idle");
            float num1 = y - 10f;
            if (_state != State.Failed)
            {
                for (int index = 0; index < 7; ++index)
                {
                    float num2 = this.x - 28f;
                    float x = num2 + index * 9 + (float)Math.Round(_scroll);
                    float num3 = num2 + 63f;
                    double num4 = (x - num2) / (num3 - num2);
                    _matchmakingSignal.depth = depth + 4;
                    if (num4 > -0.1f)
                        _matchmakingSignal.frame = 0;
                    if (num4 > 0.05f)
                        _matchmakingSignal.frame = 1;
                    if (num4 > 0.1f)
                        _matchmakingSignal.frame = 2;
                    if (num4 > 0.9f)
                        _matchmakingSignal.frame = 1;
                    if (num4 > 0.95f)
                        _matchmakingSignal.frame = 0;
                    Graphics.Draw(_matchmakingSignal, x, num1 - 21f);
                }
            }
            _matchmakingStars[0].depth = depth + 2;
            Graphics.Draw(_matchmakingStars[0], x - 9f, num1 - 18f);
            _matchmakingStars[1].depth = depth + 2;
            Graphics.Draw(_matchmakingStars[1], x + 31f, num1 - 22f);
            _matchmakingStars[2].depth = depth + 2;
            Graphics.Draw(_matchmakingStars[2], x + 12f, num1 - 20f);
            _matchmakingStars[3].depth = depth + 2;
            Graphics.Draw(_matchmakingStars[3], x - 23f, num1 - 21f);
            _signalCrossLocal.depth = depth + 2;
            Graphics.Draw(_signalCrossLocal, x - 45f, num1 - 19f);
            _signalCrossNetwork.depth = depth + 2;
            Graphics.Draw(_signalCrossNetwork, x + 55f, num1 - 23f);
            _font.DrawOutline(_caption, position + new Vec2((float)-(_font.GetWidth(_caption) / 2.0), -52f), Color.White, Color.Black, depth + 2);
            _fancyFont.scale = new Vec2(0.5f);
            int num5 = 0;
            while (messages.Count > 10)
                messages.RemoveAt(0);
            int num6 = 0;
            foreach (string message in messages)
            {
                string text = message;
                if (num6 == messages.Count - 1)
                {
                    string str1 = "";
                    if (message.EndsWith("..."))
                    {
                        string source = message.Substring(0, message.Length - 3);
                        string str2 = ".";
                        if (source.Length > 0 && source.Last() == '!' || source.Last() == '.' || source.Last() == '?')
                        {
                            str2 = source.Last().ToString() ?? "";
                            source = source.Substring(0, source.Length - 1);
                        }
                        for (int index = 0; index < 3; ++index)
                        {
                            if (_dots * 4.0 > index + 1)
                                str1 += str2;
                        }
                        text = source + str1;
                    }
                }
                _fancyFont.Draw(text, new Vec2(x - 64f, y - 18f + num5 * 6), Color.White, depth + 2);
                ++num5;
                ++num6;
            }
            if (_directConnectLobby != null)
                return;
            if (_totalLobbies >= 0)
            {
                if (_totalLobbies > 1)
                    _fancyFont.Draw("Found " + _totalLobbies.ToString() + " games already in progress.", position + new Vec2(-65f, 49f), Color.Black, depth + 2);
                else
                    _fancyFont.Draw("Found " + _totalLobbies.ToString() + " game already in progress.", position + new Vec2(-65f, 49f), Color.Black, depth + 2);
            }
            else
                _fancyFont.Draw("Querying moon...", position + new Vec2(-65f, 49f), Color.Black, depth + 2);
        }

        public class Core
        {
            public UIMatchmakerMark2 instance;
        }

        public enum State
        {
            InitializeMatchmaking,
            InitializeMatchmakingFinish,
            GetNumberOfLobbies,
            WaitForQuery,
            SearchForLobbies,
            TryJoiningLobbies,
            JoinLobby,
            PrepareLobby,
            Aborting,
            Idle,
            Failed,
            PlatformCancelled,
        }
    }
}
