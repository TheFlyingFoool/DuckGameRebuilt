// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMatchmakerMark2
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private static UIMatchmakerMark2.Core _core = new UIMatchmakerMark2.Core();
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
        protected UIMatchmakerMark2.State _state;
        protected UIMatchmakerMark2.State _previousState;
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

        public static UIMatchmakerMark2.Core core
        {
            get => UIMatchmakerMark2._core;
            set => UIMatchmakerMark2._core = value;
        }

        protected void ChangeState(UIMatchmakerMark2.State pState)
        {
            if (this._directConnectLobby != null && (pState == UIMatchmakerMark2.State.TryJoiningLobbies || pState == UIMatchmakerMark2.State.SearchForLobbies))
                pState = UIMatchmakerMark2.State.Failed;
            if (pState == UIMatchmakerMark2.State.Failed && this._previousState != UIMatchmakerMark2.State.Failed)
            {
                HUD.CloseAllCorners();
                HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@RETURN");
                this.messages.Add("|DGRED|Unable to connect to server.");
            }
            this._previousState = this._state;
            this._state = pState;
            this._timeInState = 0;
            this.Wait();
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
            this._directConnectLobby = joinLobby;
        }

        protected UIMatchmakerMark2(UIMenu openOnClose)
          : base("", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f)
        {
            this._openOnClose = openOnClose;
            Graphics.fade = 1f;
            this._window = new Sprite("online/matchmaking_mk2");
            this._window.CenterOrigin();
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

        protected virtual void Platform_Open()
        {
        }

        public override void Open()
        {
            this._state = UIMatchmakerMark2.State.InitializeMatchmaking;
            this.Platform_Open();
            this._timeOpen = 0;
            UIMatchmakerMark2._currentLevel = Level.current;
            UIMatchmakerMark2.instance = this;
            this._processing = null;
            this.messages.Clear();
            if (this._directConnectLobby != null)
                this._state = UIMatchmakerMark2.State.TryJoiningLobbies;
            else
                this.messages.Add("|DGYELLOW|Connecting to servers on the Moon...");
            this._totalLobbies = -1;
            this._joinableLobbies = -1;
            this.attempted.Clear();
            this.blacklist.Clear();
            HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@ABORT");
            if (this.playMusic && this._directConnectLobby == null)
                Music.Play("jazzroom");
            base.Open();
        }

        public override void Close()
        {
            if (UIMatchmakerMark2.instance == this)
                UIMatchmakerMark2.instance = null;
            this._state = UIMatchmakerMark2.State.Idle;
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
            this.Close();
            if (Network.isActive)
                return;
            if (this._openOnClose is UIServerBrowser)
            {
                this._openOnClose.Open();
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
            this._resetting = false;
            this._framesSinceReset = 0;
            if (this._state != UIMatchmakerMark2.State.Aborting)
            {
                if (this._hostedLobby == null)
                {
                    if (error == null || error.error != DuckNetError.HostIsABlockedUser)
                    {
                        this.messages.Add("|PURPLE|LOBBY    |DGRED|Connection to lobby failed:");
                        this.OnConnectionError(error);
                        if (this._directConnectLobby == null)
                            this.messages.Add("|PURPLE|LOBBY    |WHITE|Looking for more lobbies to try...");
                    }
                    this._wait += 60;
                }
                this.ChangeState(UIMatchmakerMark2.State.TryJoiningLobbies);
            }
            this._processing = null;
            this._hostedLobby = null;
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
                        this.messages.Add("|DGRED|Their version was older.");
                    else
                        this.messages.Add("|DGRED|Their version was newer.");
                    if (this._processing == null)
                        return;
                    this.blacklist.Add(this._processing.id);
                }
                else if (error.error == DuckNetError.FullServer)
                    this.messages.Add("|DGRED|Failed (FULL SERVER)");
                else if (error.error == DuckNetError.ConnectionTimeout)
                    this.messages.Add("|DGRED|Failed (TIMEOUT)");
                else if (error.error == DuckNetError.GameInProgress)
                    this.messages.Add("|DGRED|Failed (IN PROGRESS)");
                else if (error.error == DuckNetError.GameNotFoundOrClosed)
                    this.messages.Add("|DGRED|Failed (NO LONGER AVAILABLE)");
                else if (error.error == DuckNetError.ClientDisconnected)
                    this.messages.Add("|DGYELLOW|Disconnected");
                else if (error.error == DuckNetError.InvalidPassword)
                    this.messages.Add("|DGRED|Password was incorrect!");
                else if (error.error == DuckNetError.ModsIncompatible)
                {
                    this.messages.Add("|DGRED|Host had different mods enabled!");
                }
                else
                {
                    if (error.error == DuckNetError.HostIsABlockedUser)
                        return;
                    this.messages.Add("|DGRED|Unknown connection error.");
                    if (this._processing == null)
                        return;
                    this.blacklist.Add(this._processing.id);
                }
            }
            else
            {
                this.messages.Add("|DGRED|Connection timeout.");
                if (this._processing == null)
                    return;
                this.blacklist.Add(this._processing.id);
            }
        }

        public virtual void Hook_OnDucknetJoined()
        {
            if (Level.current is TeamSelect2)
                (Level.current as TeamSelect2).CloseAllDialogs();
            if (Network.isServer)
            {
                Level.current = new TeamSelect2();
                Level.current.suppressLevelMessage = true;
            }
            this.Close();
            DevConsole.Log("|PURPLE|LOBBY    |DGGREEN|Finished! (HOST).", Color.White);
        }

        private void Wait() => this._wait += 60;

        protected bool HostLobby()
        {
            if (this._hostedLobby == null && this.Reset())
            {
                this.messages.Add("|DGYELLOW|Having trouble finding an open lobby...");
                this.messages.Add("|DGGREEN|Creating a lobby of our very own...");
                DuckNetwork.Host(TeamSelect2.GetSettingInt("maxplayers"), NetworkLobbyType.Public);
                this._hostedLobby = Network.activeNetwork.core.lobby;
                DevConsole.Log("|PURPLE|LOBBY    |DGYELLOW|Opened lobby while searching.", Color.White);
                this._wait = 280 + Rando.Int(120);
            }
            return this._hostedLobby != null;
        }

        protected virtual void Platform_ResetLogic()
        {
        }

        protected bool Reset()
        {
            if (Network.isActive)
            {
                this.Platform_ResetLogic();
                this._resetNetwork = true;
                Network.EndNetworkingSession(new DuckNetErrorInfo(DuckNetError.ControlledDisconnect, "Matchmaking disconnect."));
                this._resetting = true;
                this._framesSinceReset = 0;
            }
            return !Network.isActive;
        }

        public static UIMatchmakerMark2 instance
        {
            get
            {
                if (UIMatchmakerMark2._currentLevel != Level.current)
                    UIMatchmakerMark2._core.instance = null;
                return UIMatchmakerMark2._core.instance;
            }
            set => UIMatchmakerMark2._core.instance = value;
        }

        public virtual void Platform_Update()
        {
        }

        public virtual void Platform_MatchmakerLogic()
        {
        }

        public override void Update()
        {
            if (!this.open)
                return;
            ++this._timeInState;
            if (UIMatchmakerMark2.instance == null)
                this.FinishAndClose();
            else if (this._resetNetwork && Network.isActive)
            {
                this.Reset();
            }
            else
            {
                this._resetNetwork = false;
                if (Input.Pressed("CANCEL"))
                {
                    this.Reset();
                    this.messages.Add("|DGRED|Aborting...");
                    this.ChangeState(UIMatchmakerMark2.State.Aborting);
                }
                if (this._resetting)
                {
                    ++this._framesSinceReset;
                    if (this._framesSinceReset <= 120)
                        return;
                    Network.Terminate();
                    if (this._state != UIMatchmakerMark2.State.Aborting)
                        this.ChangeState(UIMatchmakerMark2.State.TryJoiningLobbies);
                    this._processing = null;
                    this._hostedLobby = null;
                }
                else
                {
                    ++this._timeOpen;
                    if (this._timeOpen > 7200)
                        this.attempted.Clear();
                    this.Platform_Update();
                    if (this._wait > 0 && this._state != UIMatchmakerMark2.State.Aborting && this._state != UIMatchmakerMark2.State.JoinLobby)
                        --this._wait;
                    else
                        this.Platform_MatchmakerLogic();
                }
            }
        }

        public void SetPasswordAttempt(string pPassword) => this._passwordAttempt = pPassword;

        public override void Draw()
        {
            if (!this.open)
                return;
            this._window.depth = this.depth;
            Graphics.Draw(this._window, this.x, this.y);
            this._scroll += 0.06f;
            if (_scroll > 9.0)
                this._scroll = 0f;
            this._dots += 0.01f;
            if (_dots > 1.0)
                this._dots = 0f;
            if (this._state == UIMatchmakerMark2.State.Idle || this._state == UIMatchmakerMark2.State.Failed)
            {
                this._signalCrossLocal.SetAnimation("idle");
                UIMatchmakerMark2.pulseLocal = false;
            }
            else if (this._signalCrossLocal.currentAnimation == "idle")
            {
                if (UIMatchmakerMark2.pulseLocal)
                {
                    this._signalCrossLocal.SetAnimation("flicker");
                    UIMatchmakerMark2.pulseLocal = false;
                }
            }
            else if (this._signalCrossLocal.finished)
                this._signalCrossLocal.SetAnimation("idle");
            if (this._signalCrossNetwork.currentAnimation == "idle")
            {
                if (UIMatchmakerMark2.pulseNetwork)
                {
                    this._signalCrossNetwork.SetAnimation("flicker");
                    UIMatchmakerMark2.pulseNetwork = false;
                }
            }
            else if (this._signalCrossNetwork.finished)
                this._signalCrossNetwork.SetAnimation("idle");
            float num1 = this.y - 10f;
            if (this._state != UIMatchmakerMark2.State.Failed)
            {
                for (int index = 0; index < 7; ++index)
                {
                    float num2 = this.x - 28f;
                    float x = num2 + index * 9 + (float)Math.Round(_scroll);
                    float num3 = num2 + 63f;
                    double num4 = ((double)x - (double)num2) / ((double)num3 - (double)num2);
                    this._matchmakingSignal.depth = this.depth + 4;
                    if (num4 > -0.100000001490116)
                        this._matchmakingSignal.frame = 0;
                    if (num4 > 0.0500000007450581)
                        this._matchmakingSignal.frame = 1;
                    if (num4 > 0.100000001490116)
                        this._matchmakingSignal.frame = 2;
                    if (num4 > 0.899999976158142)
                        this._matchmakingSignal.frame = 1;
                    if (num4 > 0.949999988079071)
                        this._matchmakingSignal.frame = 0;
                    Graphics.Draw(_matchmakingSignal, x, num1 - 21f);
                }
            }
            this._matchmakingStars[0].depth = this.depth + 2;
            Graphics.Draw(this._matchmakingStars[0], this.x - 9f, num1 - 18f);
            this._matchmakingStars[1].depth = this.depth + 2;
            Graphics.Draw(this._matchmakingStars[1], this.x + 31f, num1 - 22f);
            this._matchmakingStars[2].depth = this.depth + 2;
            Graphics.Draw(this._matchmakingStars[2], this.x + 12f, num1 - 20f);
            this._matchmakingStars[3].depth = this.depth + 2;
            Graphics.Draw(this._matchmakingStars[3], this.x - 23f, num1 - 21f);
            this._signalCrossLocal.depth = this.depth + 2;
            Graphics.Draw(_signalCrossLocal, this.x - 45f, num1 - 19f);
            this._signalCrossNetwork.depth = this.depth + 2;
            Graphics.Draw(_signalCrossNetwork, this.x + 55f, num1 - 23f);
            this._font.DrawOutline(this._caption, this.position + new Vec2((float)-((double)this._font.GetWidth(this._caption) / 2.0), -52f), Color.White, Color.Black, this.depth + 2);
            this._fancyFont.scale = new Vec2(0.5f);
            int num5 = 0;
            while (this.messages.Count > 10)
                this.messages.RemoveAt(0);
            int num6 = 0;
            foreach (string message in this.messages)
            {
                string text = message;
                if (num6 == this.messages.Count - 1)
                {
                    string str1 = "";
                    if (message.EndsWith("..."))
                    {
                        string source = message.Substring(0, message.Length - 3);
                        string str2 = ".";
                        if (source.Count<char>() > 0 && source.Last<char>() == '!' || source.Last<char>() == '.' || source.Last<char>() == '?')
                        {
                            str2 = source.Last<char>().ToString() ?? "";
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
                this._fancyFont.Draw(text, new Vec2(this.x - 64f, this.y - 18f + num5 * 6), Color.White, this.depth + 2);
                ++num5;
                ++num6;
            }
            if (this._directConnectLobby != null)
                return;
            if (this._totalLobbies >= 0)
            {
                if (this._totalLobbies > 1)
                    this._fancyFont.Draw("Found " + this._totalLobbies.ToString() + " games already in progress.", this.position + new Vec2(-65f, 49f), Color.Black, this.depth + 2);
                else
                    this._fancyFont.Draw("Found " + this._totalLobbies.ToString() + " game already in progress.", this.position + new Vec2(-65f, 49f), Color.Black, this.depth + 2);
            }
            else
                this._fancyFont.Draw("Querying moon...", this.position + new Vec2(-65f, 49f), Color.Black, this.depth + 2);
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
