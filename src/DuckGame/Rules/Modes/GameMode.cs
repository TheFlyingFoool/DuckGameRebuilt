// Decompiled with JetBrains decompiler
// Type: DuckGame.GameMode
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DuckGame
{
    public class GameMode
    {
        private static GameModeCore _core = new GameModeCore();
        public static List<Profile> lastWinners = new List<Profile>();
        protected UIComponent _pauseGroup;
        protected UIMenu _pauseMenu;
        protected UIMenu _confirmMenu;
        protected UIMenu _confirmBlacklistMenu;
        protected UIMenu _confirmReturnToLobby;
        protected MenuBoolean _returnToLobby = new MenuBoolean();
        protected bool _matchOver;
        protected bool _paused;
        protected MenuBoolean _quit = new MenuBoolean();
        protected float _wait;
        protected float _roundEndWait = 1f;
        protected bool _doScore;
        protected bool _addedPoints;
        protected bool _endedHighlights;
        protected bool _switchedLevel;
        protected bool _roundHadWinner;
        protected float _waitFade = 1f;
        protected float _waitSpawn = 1.8f;
        protected float _waitAfterSpawn;
        protected int _waitAfterSpawnDings;
        protected float _fontFade = 1f;
        protected List<Duck> _pendingSpawns;
        protected BitmapFont _font;
        protected bool _editorTestMode;
        protected bool _validityTest;
        private Stopwatch _watch;
        private long frames;
        public bool skippedLevel;
        protected float _pulse;

        public static GameModeCore core
        {
            get => GameMode._core;
            set => GameMode._core = value;
        }

        public static int roundsBetweenIntermission
        {
            get => GameMode._core.roundsBetweenIntermission;
            set => GameMode._core.roundsBetweenIntermission = value;
        }

        public static int winsPerSet
        {
            get => GameMode._core.winsPerSet;
            set => GameMode._core.winsPerSet = value;
        }

        protected static bool _started
        {
            get => GameMode._core._started;
            set => GameMode._core._started = value;
        }

        public static bool started => GameMode._started;

        public static bool getReady
        {
            get => GameMode._core.getReady;
            set => GameMode._core.getReady = value;
        }

        protected static int _numMatchesPlayed
        {
            get => GameMode._core._numMatchesPlayed;
            set => GameMode._core._numMatchesPlayed = value;
        }

        public static int numMatchesPlayed
        {
            get => GameMode._numMatchesPlayed;
            set => GameMode._numMatchesPlayed = value;
        }

        public static bool showdown
        {
            get => GameMode._core.showdown;
            set => GameMode._core.showdown = value;
        }

        public static string previousLevel
        {
            get => GameMode._core.previousLevel;
            set => GameMode._core.previousLevel = value;
        }

        protected static string _currentMusic
        {
            get => GameMode._core._currentMusic;
            set => GameMode._core._currentMusic = value;
        }

        public static bool firstDead
        {
            get => GameMode._core.firstDead;
            set => GameMode._core.firstDead = value;
        }

        public static bool playedGame
        {
            get => GameMode._core.playedGame;
            set => GameMode._core.playedGame = value;
        }

        public bool matchOver => this._matchOver;

        public List<Duck> pendingSpawns
        {
            get => this._pendingSpawns;
            set => this._pendingSpawns = value;
        }

        public GameMode(bool validityTest = false, bool editorTestMode = false)
        {
            this._validityTest = validityTest;
            this._editorTestMode = editorTestMode;
        }

        public static void Subscribe()
        {
            if (!(Level.current is GameLevel) || (Level.current as GameLevel).data == null || (Level.current as GameLevel).data.metaData.workshopID == 0UL)
                return;
            WorkshopItem workshopItem = WorkshopItem.GetItem((Level.current as GameLevel).data.metaData.workshopID);
            if ((workshopItem.stateFlags & WorkshopItemState.Subscribed) != WorkshopItemState.None)
                Steam.WorkshopUnsubscribe(workshopItem.id);
            else
                Steam.WorkshopSubscribe(workshopItem.id);
        }

        public static void View()
        {
            if (!(Level.current is GameLevel) || (Level.current as GameLevel).data == null || (Level.current as GameLevel).data.metaData.workshopID == 0UL)
                return;
            Steam.OverlayOpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + (Level.current as GameLevel).data.metaData.workshopID.ToString());
        }

        public static void Blacklist()
        {
            if (Level.current is GameLevel && (Level.current as GameLevel).data != null && (Level.current as GameLevel).data.metaData.workshopID != 0UL)
                Global.data.blacklist.Add((Level.current as GameLevel).data.metaData.workshopID);
            GameMode.Skip();
        }

        public static void Skip()
        {
            if (!(Level.current is GameLevel))
                return;
            (Level.current as GameLevel).SkipMatch();
        }

        public void DoInitialize()
        {
            GameMode._started = false;
            GameMode.firstDead = false;
            GameMode.playedGame = true;
            if (!this._editorTestMode)
                Highlights.StartRound();
            this._font = new BitmapFont("biosFont", 8);
            if (Network.isServer)
            {
                string str = Music.RandomTrack("InGame", Music.currentSong);
                Music.LoadAlternateSong(str);
                Music.CancelLooping();
                if (Network.isActive)
                    Send.Message(new NMSwitchMusic(str));
            }
            this.Initialize();
            if (Network.isActive)
                GameMode.getReady = false;
            else
                GameMode.getReady = true;
        }

        private void CreatePauseGroup()
        {
            if (this._pauseGroup != null)
                Level.Remove(_pauseGroup);
            if (this._editorTestMode)
            {
                this._pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
                this._pauseMenu = new UIMenu("@LWING@PAUSE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
                this._confirmMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
                UIDivider component = new UIDivider(true, 0.8f);
                component.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(this._pauseGroup), UIAlign.Left), true);
                component.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
                component.leftSection.Add(new UIText(" ", Color.White, UIAlign.Left), true);
                component.leftSection.Add(new UIMenuItem("|DGRED|QUIT", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._quit), UIAlign.Left), true);
                component.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
                this._pauseMenu.Add(component, true);
                this._pauseMenu.Close();
                this._pauseGroup.Add(_pauseMenu, false);
                Options.AddMenus(this._pauseGroup);
                Options.openOnClose = this._pauseMenu;
                this._confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_confirmMenu, _pauseMenu), UIAlign.Left, backButton: true), true);
                this._confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._quit)), true);
                this._confirmMenu.Close();
                this._pauseGroup.Add(_confirmMenu, false);
                this._pauseGroup.isPauseMenu = true;
                Level.Add(_pauseGroup);
                this._pauseGroup.Update();
                this._pauseGroup.Update();
                this._pauseGroup.Close();
                this._confirmMenu.Update();
            }
            else
            {
                this._pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
                this._pauseMenu = new UIMenu("@LWING@PAUSE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
                this._confirmMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
                this._confirmBlacklistMenu = new UIMenu("AVOID LEVEL?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
                this._confirmReturnToLobby = new UIMenu("RETURN TO LOBBY?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@CANCEL@BACK @SELECT@SELECT");
                UIDivider component1 = new UIDivider(true, 0.8f);
                component1.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(this._pauseGroup), UIAlign.Left), true);
                component1.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
                component1.leftSection.Add(new UIText(" ", Color.White, UIAlign.Left), true);
                component1.leftSection.Add(new UIMenuItem("|DGRED|BACK TO LOBBY", new UIMenuActionOpenMenu(_pauseMenu, _confirmReturnToLobby), UIAlign.Left), true);
                component1.leftSection.Add(new UIMenuItem("|DGRED|QUIT", new UIMenuActionOpenMenu(_pauseMenu, _confirmMenu), UIAlign.Left), true);
                this._confirmReturnToLobby.Add(new UIText("YOUR CURRENT GAME", Color.White), true);
                this._confirmReturnToLobby.Add(new UIText("WILL BE CANCELLED.", Color.White), true);
                this._confirmReturnToLobby.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_confirmReturnToLobby, _pauseMenu), UIAlign.Left, backButton: true), true);
                this._confirmReturnToLobby.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._returnToLobby)), true);
                this._confirmReturnToLobby.Close();
                this._pauseGroup.Add(_confirmReturnToLobby, false);
                if (Level.current.isCustomLevel || Level.current is RandomLevel)
                {
                    component1.leftSection.Add(new UIText(" ", Color.White), true);
                    if ((Level.current as GameLevel).data.metaData.workshopID != 0UL)
                    {
                        WorkshopItem workshopItem = WorkshopItem.GetItem((Level.current as GameLevel).data.metaData.workshopID);
                        if (workshopItem != null)
                        {
                            component1.leftSection.Add(new UIMenuItem("@STEAMICON@|DGGREEN|VIEW", new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(GameMode.View)), UIAlign.Left), true);
                            if ((workshopItem.stateFlags & WorkshopItemState.Subscribed) != WorkshopItemState.None)
                            {
                                component1.leftSection.Add(new UIMenuItem("@STEAMICON@|DGRED|UNSUBSCRIBE", new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Subscribe)), UIAlign.Left), true);
                            }
                            else
                            {
                                component1.leftSection.Add(new UIMenuItem("@STEAMICON@|DGGREEN|SUBSCRIBE", new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Subscribe)), UIAlign.Left), true);
                                component1.leftSection.Add(new UIMenuItem("@blacklist@|DGRED|NEVER AGAIN", new UIMenuActionOpenMenu(_pauseMenu, _confirmBlacklistMenu), UIAlign.Left), true);
                            }
                        }
                    }
                    if (!this._matchOver && Network.isServer)
                        component1.leftSection.Add(new UIMenuItem("@SKIPSPIN@|DGRED|SKIP", new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Skip)), UIAlign.Left), true);
                }
                component1.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
                this._pauseMenu.Add(component1, true);
                this._pauseMenu.Close();
                this._pauseGroup.Add(_pauseMenu, false);
                Options.AddMenus(this._pauseGroup);
                Options.openOnClose = this._pauseMenu;
                this._confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_confirmMenu, _pauseMenu), UIAlign.Left, backButton: true), true);
                this._confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._quit)), true);
                this._confirmMenu.Close();
                this._pauseGroup.Add(_confirmMenu, false);
                UIMenu confirmBlacklistMenu1 = this._confirmBlacklistMenu;
                UIText component2 = new UIText("", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                confirmBlacklistMenu1.Add(component2, true);
                UIMenu confirmBlacklistMenu2 = this._confirmBlacklistMenu;
                UIText component3 = new UIText("Are you sure you want to avoid", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                confirmBlacklistMenu2.Add(component3, true);
                UIMenu confirmBlacklistMenu3 = this._confirmBlacklistMenu;
                UIText component4 = new UIText("this level in the future?", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                confirmBlacklistMenu3.Add(component4, true);
                UIMenu confirmBlacklistMenu4 = this._confirmBlacklistMenu;
                UIText component5 = new UIText("", Color.White, heightAdd: -4f)
                {
                    scale = new Vec2(0.5f)
                };
                confirmBlacklistMenu4.Add(component5, true);
                this._confirmBlacklistMenu.Add(new UIMenuItem("|DGRED|@blacklist@YES!", new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Blacklist))), true);
                this._confirmBlacklistMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_confirmBlacklistMenu, _pauseMenu), backButton: true), true);
                this._confirmBlacklistMenu.Close();
                this._pauseGroup.Add(_confirmBlacklistMenu, false);
                Level.Add(_pauseGroup);
                this._pauseGroup.isPauseMenu = true;
                this._pauseGroup.Close();
                this._pauseGroup.Update();
                this._pauseGroup.Update();
                this._confirmBlacklistMenu.Update();
                this._confirmMenu.Update();
                this._pauseMenu.Update();
                this._confirmReturnToLobby.Update();
                this._confirmBlacklistMenu.Update();
            }
        }

        protected virtual void Initialize() => Graphics.fade = 1f;

        public void DoStart()
        {
            if (!this._editorTestMode)
            {
                ++Deathmatch.levelsSinceRandom;
                ++Deathmatch.levelsSinceWorkshop;
                ++Deathmatch.levelsSinceCustom;
                foreach (Profile profile in Profiles.active)
                    ++profile.stats.timesSpawned;
                ++Global.data.timesSpawned.valueInt;
            }
            this.Start();
            GameMode._started = true;
        }

        protected virtual void Start()
        {
        }

        public void DoUpdate()
        {
            if (this._validityTest)
            {
                if (this._watch == null)
                    this._watch = new Stopwatch();
                this._watch.Start();
            }
            if (Graphics.fade > 0.9f && Input.Pressed("START") && !Network.isActive)
            {
                if (this._watch != null)
                    this._watch.Stop();
                this.CreatePauseGroup();
                this._pauseGroup.DoUpdate();
                this._pauseGroup.DoUpdate();
                this._pauseGroup.DoUpdate();
                this._pauseMenu.DoUpdate();
                this._pauseGroup.Open();
                this._pauseMenu.Open();
                MonoMain.pauseMenu = this._pauseGroup;
                if (this._paused)
                    return;
                if (!this._validityTest)
                    Music.Pause();
                SFX.Play("pause", 0.6f);
                this._paused = true;
            }
            else
            {
                if (this._paused && MonoMain.pauseMenu == null)
                {
                    this._paused = false;
                    SFX.Play("resume", 0.6f);
                    if (!this._validityTest)
                        Music.Resume();
                }
                if (this._returnToLobby.value && !Network.isActive)
                {
                    Level.current = new TeamSelect2(true);
                    this._returnToLobby.value = false;
                }
                if (this._quit.value)
                {
                    if (this._editorTestMode)
                        Level.current = DuckGameTestArea.currentEditor;
                    else if (this._validityTest)
                        Level.current = DeathmatchTestDialogue.currentEditor;
                    else if (Network.isActive)
                    {
                        Level.current = new DisconnectFromGame();
                    }
                    else
                    {
                        Graphics.fade -= 0.04f;
                        if (Graphics.fade >= 0.01f)
                            return;
                        Level.current = new TitleScreen();
                    }
                }
                else
                {
                    Graphics.fade = 1f;
                    if (!this._validityTest && Music.finished)
                    {
                        if (Music.pendingSong != null)
                            Music.SwitchSongs();
                        else
                            this.PlayMusic();
                    }
                    if (Music.finished)
                        this._wait -= 0.0006f;
                    this._waitFade -= 0.04f;
                    if (_waitFade > 0.0 || !GameMode.getReady || Network.isActive && Network.isClient && !Level.current.transferCompleteCalled)
                        return;
                    this._waitSpawn -= 0.06f;
                    if (_waitSpawn <= 0.0)
                    {
                        if (Network.isServer && this._pendingSpawns == null)
                            this._pendingSpawns = this.AssignSpawns();
                        if (this._pendingSpawns != null && this._pendingSpawns.Count > 0)
                        {
                            this._waitSpawn = 1.1f;
                            if (this._pendingSpawns.Count == 1)
                                this._waitSpawn = 2f;
                            Duck pendingSpawn = this._pendingSpawns[0];
                            pendingSpawn.respawnPos = pendingSpawn.position;
                            pendingSpawn.localSpawnVisible = true;
                            this._pendingSpawns.RemoveAt(0);
                            Vec3 color = pendingSpawn.profile.persona.color;
                            Level.Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, 0f, new Color((int)color.x, (int)color.y, (int)color.z), 32f));
                            Level.Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, -4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                            Level.Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                            Level.Add(new SpawnAimer(pendingSpawn.x, pendingSpawn.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), pendingSpawn.persona, 4f)
                            {
                                dugg = pendingSpawn
                            });
                            SFX.Play("pullPin", 0.7f);
                            if (pendingSpawn.isServerForObject && !this._editorTestMode)
                            {
                                if (!Network.isActive && pendingSpawn.profile.team.name == "ZEKE")
                                {
                                    Ragdoll ragdoll = new Ragdoll(pendingSpawn.x, pendingSpawn.y, null, false, 0f, 0, Vec2.Zero);
                                    Level.Add(ragdoll);
                                    ragdoll.RunInit();
                                    ragdoll.MakeZekeBear();
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Present) || TeamSelect2.Enabled("WINPRES") && GameMode.lastWinners.Contains(pendingSpawn.profile))
                                {
                                    Present h = new Present(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(h);
                                    pendingSpawn.GiveHoldable(h);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Jetpack) || TeamSelect2.Enabled("JETTY"))
                                {
                                    Jetpack e = new Jetpack(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e);
                                    pendingSpawn.Equip(e);
                                }
                                if (TeamSelect2.Enabled("HELMY"))
                                {
                                    Helmet e = new Helmet(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e);
                                    pendingSpawn.Equip(e);
                                }
                                if (TeamSelect2.Enabled("SHOESTAR"))
                                {
                                    Boots e = new Boots(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e);
                                    pendingSpawn.Equip(e);
                                }
                                if (DevConsole.fancyMode)
                                {
                                    FancyShoes e = new FancyShoes(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e);
                                    pendingSpawn.Equip(e);
                                }
                                if (TeamSelect2.Enabled("DILLY"))
                                {
                                    DuelingPistol h = new DuelingPistol(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(h);
                                    pendingSpawn.GiveHoldable(h);
                                }
                                if (TeamSelect2.Enabled("COOLBOOK"))
                                {
                                    GoodBook h = new GoodBook(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(h);
                                    pendingSpawn.GiveHoldable(h);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Armor))
                                {
                                    Helmet e1 = new Helmet(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e1);
                                    pendingSpawn.Equip(e1);
                                    ChestPlate e2 = new ChestPlate(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e2);
                                    pendingSpawn.Equip(e2);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Pistol))
                                {
                                    Pistol h = new Pistol(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(h);
                                    pendingSpawn.GiveHoldable(h);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.NetGun))
                                {
                                    NetGun h = new NetGun(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(h);
                                    pendingSpawn.GiveHoldable(h);
                                }
                                if (TeamSelect2.QUACK3)
                                {
                                    Helmet e3 = new Helmet(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e3);
                                    pendingSpawn.Equip(e3);
                                    ChestPlate e4 = new ChestPlate(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e4);
                                    pendingSpawn.Equip(e4);
                                    Holster e5 = new Holster(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add(e5);
                                    pendingSpawn.Equip(e5);
                                    if (pendingSpawn.profile.carryOverObject != null)
                                    {
                                        Level.Add(pendingSpawn.profile.carryOverObject);
                                        e5.SetContainedObject(pendingSpawn.profile.carryOverObject);
                                    }
                                    else
                                    {
                                        DuelingPistol h = new DuelingPistol(pendingSpawn.x, pendingSpawn.y);
                                        Level.Add(h);
                                        e5.SetContainedObject(h);
                                    }
                                }
                            }
                        }
                        else if (!GameMode._started)
                        {
                            this._waitAfterSpawn -= 0.05f;
                            if (_waitAfterSpawn <= 0.0)
                            {
                                ++this._waitAfterSpawnDings;
                                if (this._waitAfterSpawnDings > 2)
                                {
                                    Party.Clear();
                                    this.DoStart();
                                    SFX.Play("ding");
                                    Event.Log(new RoundStartEvent());
                                    GameMode.lastWinners.Clear();
                                    if (Level.current is GameLevel)
                                        (Level.current as GameLevel).MatchStart();
                                    foreach (Duck duck in Level.current.things[typeof(Duck)])
                                    {
                                        if (duck.profile.localPlayer)
                                            duck.connection = DuckNetwork.localConnection;
                                        duck.immobilized = false;
                                    }
                                }
                                else
                                    SFX.Play("preStartDing");
                                this._waitSpawn = 1.1f;
                            }
                        }
                        else
                        {
                            this._fontFade -= 0.1f;
                            if (_fontFade < 0.0)
                                this._fontFade = 0f;
                        }
                    }
                    if (Network.isClient)
                        return;
                    if (GameMode._started)
                        this.Update();
                    if (this._matchOver)
                        this._roundEndWait -= 0.005f;
                    if (this.skippedLevel)
                        this._roundEndWait = -1f;
                    if (_roundEndWait < 0.5f && !this._addedPoints && !this.skippedLevel)
                        this.DoAddPoints();
                    if (_roundEndWait < 0.1f && !this._endedHighlights)
                    {
                        this._endedHighlights = true;
                        if (!this._editorTestMode)
                            Highlights.FinishRound();
                    }
                    if (_roundEndWait >= 0f || this._switchedLevel)
                        return;
                    bool flag = false;
                    if (!Network.isActive && !this.skippedLevel)
                    {
                        int num = 0;
                        List<Team> winning = Teams.winning;
                        if (winning.Count > 0)
                        {
                            num = winning[0].score;
                            if (Teams.active.Count > 1)
                                Global.WinLevel(winning[0]);
                        }
                        else
                        {
                            flag = true;
                            if (Teams.active.Count > 1)
                                Global.WinLevel(null);
                        }
                        if (!this._editorTestMode && num > 4)
                        {
                            foreach (Team team in Teams.active)
                            {
                                if (team.score != num)
                                {
                                    if (team.score < 1)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < 2 && Rando.Float(1f) > 0.3f)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < 5 && Rando.Float(1f) > 0.6f)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < 7 && Rando.Float(1f) > 0.85f)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < num && Rando.Float(1f) > 0.9f)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                }
                            }
                        }
                    }
                    Level nextLevel = this.GetNextLevel();
                    GameMode.previousLevel = nextLevel.level;
                    if (!this.skippedLevel)
                    {
                        if (Teams.active.Count > 1)
                        {
                            if (!this._editorTestMode)
                                ++Global.data.levelsPlayed;
                            if (flag && Network.isServer)
                            {
                                if (!this._editorTestMode)
                                    ++Global.data.littleDraws.valueInt;
                                if (Network.isActive)
                                    Send.Message(new NMAssignDraw());
                            }
                        }
                        if (Network.isServer)
                        {
                            List<int> scrs = new List<int>();
                            foreach (Profile profile in DuckNetwork.profiles)
                            {
                                profile.ready = true;
                                if (profile.team != null)
                                {
                                    scrs.Add(profile.team.score);
                                    if (profile.connection != null)
                                        profile.ready = false;
                                }
                                else
                                    scrs.Add(0);
                            }
                            Send.Message(new NMTransferScores(scrs));
                            GameMode.RunPostRound(this._editorTestMode);
                        }
                    }
                    if (this._validityTest && this._watch != null)
                    {
                        long elapsedMilliseconds = this._watch.ElapsedMilliseconds;
                        if (frames / (_watch.ElapsedMilliseconds / 1000.0) < 30.0)
                        {
                            DeathmatchTestDialogue.success = false;
                            DeathmatchTestDialogue.tooSlow = true;
                        }
                        else
                            DeathmatchTestDialogue.success = true;
                        Level.current = DeathmatchTestDialogue.currentEditor;
                    }
                    else
                    {
                        if (TeamSelect2.QUACK3)
                        {
                            foreach (Profile profile in Profiles.active)
                                profile.carryOverObject = null;
                            foreach (Duck duck in Level.current.things[typeof(Duck)])
                            {
                                if (duck.GetEquipment(typeof(Holster)) is Holster equipment && equipment.containedObject != null)
                                    duck.profile.carryOverObject = equipment.containedObject;
                            }
                        }
                        if (!DuckNetwork.TryPeacefulResolution())
                        {
                            if (this._doScore && !this.skippedLevel)
                            {
                                this._doScore = false;
                                if (GameMode.showdown)
                                {
                                    if (this._roundHadWinner)
                                    {
                                        GameMode.showdown = false;
                                        Level.current = new RockScoreboard(nextLevel, ScoreBoardMode.ShowWinner);
                                        if (!this._editorTestMode)
                                            ++Global.data.drawsPlayed.valueInt;
                                        if (Network.isActive)
                                            Send.Message(new NMDrawBroken());
                                    }
                                    else
                                    {
                                        this._endedHighlights = false;
                                        Level.current = nextLevel;
                                    }
                                }
                                else
                                {
                                    Level.current = new RockIntro(nextLevel);
                                    this._doScore = false;
                                }
                            }
                            else
                            {
                                this._endedHighlights = false;
                                Level.current = !TeamSelect2.partyMode || this.skippedLevel ? nextLevel : new DrinkRoom(nextLevel);
                            }
                        }
                        this._switchedLevel = true;
                    }
                }
            }
        }

        public static void RunPostRound(bool testMode)
        {
            if (Level.current == null | testMode)
                return;
            if (!Global.HasAchievement("mine"))
            {
                foreach (Mine mine in Level.current.things[typeof(Mine)])
                {
                    foreach (KeyValuePair<Duck, float> keyValuePair in mine.ducksOnMine)
                    {
                        if (!keyValuePair.Key.dead && (double)keyValuePair.Value > 5.0 && keyValuePair.Key.profile != null && (!Network.isActive || keyValuePair.Key.profile.connection == DuckNetwork.localConnection))
                        {
                            Global.GiveAchievement("mine");
                            break;
                        }
                    }
                    if (Global.HasAchievement("mine"))
                        break;
                }
            }
            if (!Global.HasAchievement("book"))
            {
                int num = 0;
                foreach (Duck duck in Level.current.things[typeof(Duck)])
                {
                    if (duck.converted != null && duck.converted.profile != null && (!Network.isActive || duck.converted.profile.connection == DuckNetwork.localConnection))
                        ++num;
                }
                if (num > 2)
                    Global.GiveAchievement("book");
            }
            if (Teams.active.Count <= 1 || Profiles.experienceProfile == null || Network.isActive && (DuckNetwork.localProfile == null || DuckNetwork.localProfile.slotType == SlotType.Spectator))
                return;
            DuckNetwork.GiveXP("Rounds", 1, Rando.Int(6, 7), firstCap: 350, secondCap: 650);
            if (Profiles.experienceProfile.roundsSinceXP > 10)
                DuckNetwork.GiveXP("Participation", 0, 75, firstCap: 75, secondCap: 75, finalCap: 75);
            ++Profiles.experienceProfile.roundsSinceXP;
        }

        protected virtual void Update()
        {
        }

        public List<Duck> PrepareSpawns()
        {
            this._pendingSpawns = this.AssignSpawns();
            return this._pendingSpawns;
        }

        protected virtual List<Duck> AssignSpawns() => new List<Duck>();

        protected virtual void PlayMusic()
        {
            if (this._validityTest)
                return;
            string music = Music.RandomTrack("InGame", GameMode._currentMusic);
            Music.Play(music, false);
            GameMode._currentMusic = music;
        }

        protected virtual Level GetNextLevel() => this._editorTestMode ? new GameLevel((Level.current as GameLevel).levelInputString, editorTestMode: true) : (Level)new GameLevel(Deathmatch.RandomLevelString(GameMode.previousLevel));

        protected void DoAddPoints()
        {
            this._addedPoints = true;
            Event.Log(new RoundEndEvent());
            Highlights.highlightRatingMultiplier = 0f;
            if (this.AddPoints().Count > 0)
            {
                this._roundHadWinner = true;
                SFX.Play("scoreDing", 0.9f);
            }
            if (this.skippedLevel || this._editorTestMode)
                return;
            ++GameMode._numMatchesPlayed;
            if (GameMode._numMatchesPlayed < GameMode.roundsBetweenIntermission && !GameMode.showdown)
                return;
            GameMode._numMatchesPlayed = 0;
            this._doScore = true;
        }

        protected virtual List<Profile> AddPoints() => new List<Profile>();

        public void SkipMatch()
        {
            this.skippedLevel = true;
            this.EndMatch();
        }

        protected void EndMatch() => this._matchOver = true;

        public virtual void PostDrawLayer(Layer layer)
        {
            ++this.frames;
            if (layer != Layer.HUD)
                return;
            if (this._waitAfterSpawnDings > 0 && _fontFade > 0.01f)
            {
                this._font.scale = new Vec2(2f, 2f);
                this._font.alpha = this._fontFade;
                string text = "GET";
                if (this._waitAfterSpawnDings == 2)
                    text = "READY";
                else if (this._waitAfterSpawnDings == 3)
                    text = "";
                float width = this._font.GetWidth(text);
                this._font.Draw(text, (float)((double)Layer.HUD.camera.width / 2.0 - (double)width / 2.0), (float)((double)Layer.HUD.camera.height / 2.0 - (double)this._font.height / 2.0), Color.White);
            }
            if (!this._validityTest || this._waitAfterSpawnDings <= 0 || _fontFade >= 0.5)
                return;
            this._pulse += 0.08f;
            string text1 = "WIN THE MATCH";
            float width1 = this._font.GetWidth(text1);
            this._font.alpha = (float)((Math.Sin(_pulse) + 1.0) / 2.0);
            this._font.Draw(text1, (float)((double)Layer.HUD.camera.width / 2.0 - (double)width1 / 2.0), (float)((double)Layer.HUD.camera.height - (double)this._font.height - 16.0), Color.Red);
        }
    }
}
