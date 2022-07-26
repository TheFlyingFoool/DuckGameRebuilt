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
                    Send.Message((NetMessage)new NMSwitchMusic(str));
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
                Level.Remove((Thing)this._pauseGroup);
            if (this._editorTestMode)
            {
                this._pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f);
                this._pauseMenu = new UIMenu("@LWING@PAUSE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
                this._confirmMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
                UIDivider component = new UIDivider(true, 0.8f);
                component.leftSection.Add((UIComponent)new UIMenuItem("RESUME", (UIMenuAction)new UIMenuActionCloseMenu(this._pauseGroup), UIAlign.Left), true);
                component.leftSection.Add((UIComponent)new UIMenuItem("OPTIONS", (UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._pauseMenu, (UIComponent)Options.optionsMenu), UIAlign.Left), true);
                component.leftSection.Add((UIComponent)new UIText(" ", Color.White, UIAlign.Left), true);
                component.leftSection.Add((UIComponent)new UIMenuItem("|DGRED|QUIT", (UIMenuAction)new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._quit), UIAlign.Left), true);
                component.rightSection.Add((UIComponent)new UIImage("pauseIcons", UIAlign.Right), true);
                this._pauseMenu.Add((UIComponent)component, true);
                this._pauseMenu.Close();
                this._pauseGroup.Add((UIComponent)this._pauseMenu, false);
                Options.AddMenus(this._pauseGroup);
                Options.openOnClose = this._pauseMenu;
                this._confirmMenu.Add((UIComponent)new UIMenuItem("NO!", (UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._confirmMenu, (UIComponent)this._pauseMenu), UIAlign.Left, backButton: true), true);
                this._confirmMenu.Add((UIComponent)new UIMenuItem("YES!", (UIMenuAction)new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._quit)), true);
                this._confirmMenu.Close();
                this._pauseGroup.Add((UIComponent)this._confirmMenu, false);
                this._pauseGroup.isPauseMenu = true;
                Level.Add((Thing)this._pauseGroup);
                this._pauseGroup.Update();
                this._pauseGroup.Update();
                this._pauseGroup.Close();
                this._confirmMenu.Update();
            }
            else
            {
                this._pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f);
                this._pauseMenu = new UIMenu("@LWING@PAUSE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE @SELECT@SELECT");
                this._confirmMenu = new UIMenu("REALLY QUIT?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
                this._confirmBlacklistMenu = new UIMenu("AVOID LEVEL?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK @SELECT@SELECT");
                this._confirmReturnToLobby = new UIMenu("RETURN TO LOBBY?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@CANCEL@BACK @SELECT@SELECT");
                UIDivider component1 = new UIDivider(true, 0.8f);
                component1.leftSection.Add((UIComponent)new UIMenuItem("RESUME", (UIMenuAction)new UIMenuActionCloseMenu(this._pauseGroup), UIAlign.Left), true);
                component1.leftSection.Add((UIComponent)new UIMenuItem("OPTIONS", (UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._pauseMenu, (UIComponent)Options.optionsMenu), UIAlign.Left), true);
                component1.leftSection.Add((UIComponent)new UIText(" ", Color.White, UIAlign.Left), true);
                component1.leftSection.Add((UIComponent)new UIMenuItem("|DGRED|BACK TO LOBBY", (UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._pauseMenu, (UIComponent)this._confirmReturnToLobby), UIAlign.Left), true);
                component1.leftSection.Add((UIComponent)new UIMenuItem("|DGRED|QUIT", (UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._pauseMenu, (UIComponent)this._confirmMenu), UIAlign.Left), true);
                this._confirmReturnToLobby.Add((UIComponent)new UIText("YOUR CURRENT GAME", Color.White), true);
                this._confirmReturnToLobby.Add((UIComponent)new UIText("WILL BE CANCELLED.", Color.White), true);
                this._confirmReturnToLobby.Add((UIComponent)new UIMenuItem("NO!", (UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._confirmReturnToLobby, (UIComponent)this._pauseMenu), UIAlign.Left, backButton: true), true);
                this._confirmReturnToLobby.Add((UIComponent)new UIMenuItem("YES!", (UIMenuAction)new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._returnToLobby)), true);
                this._confirmReturnToLobby.Close();
                this._pauseGroup.Add((UIComponent)this._confirmReturnToLobby, false);
                if (Level.current.isCustomLevel || Level.current is RandomLevel)
                {
                    component1.leftSection.Add((UIComponent)new UIText(" ", Color.White), true);
                    if ((Level.current as GameLevel).data.metaData.workshopID != 0UL)
                    {
                        WorkshopItem workshopItem = WorkshopItem.GetItem((Level.current as GameLevel).data.metaData.workshopID);
                        if (workshopItem != null)
                        {
                            component1.leftSection.Add((UIComponent)new UIMenuItem("@STEAMICON@|DGGREEN|VIEW", (UIMenuAction)new UIMenuActionCallFunction(new UIMenuActionCallFunction.Function(GameMode.View)), UIAlign.Left), true);
                            if ((workshopItem.stateFlags & WorkshopItemState.Subscribed) != WorkshopItemState.None)
                            {
                                component1.leftSection.Add((UIComponent)new UIMenuItem("@STEAMICON@|DGRED|UNSUBSCRIBE", (UIMenuAction)new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Subscribe)), UIAlign.Left), true);
                            }
                            else
                            {
                                component1.leftSection.Add((UIComponent)new UIMenuItem("@STEAMICON@|DGGREEN|SUBSCRIBE", (UIMenuAction)new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Subscribe)), UIAlign.Left), true);
                                component1.leftSection.Add((UIComponent)new UIMenuItem("@blacklist@|DGRED|NEVER AGAIN", (UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._pauseMenu, (UIComponent)this._confirmBlacklistMenu), UIAlign.Left), true);
                            }
                        }
                    }
                    if (!this._matchOver && Network.isServer)
                        component1.leftSection.Add((UIComponent)new UIMenuItem("@SKIPSPIN@|DGRED|SKIP", (UIMenuAction)new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Skip)), UIAlign.Left), true);
                }
                component1.rightSection.Add((UIComponent)new UIImage("pauseIcons", UIAlign.Right), true);
                this._pauseMenu.Add((UIComponent)component1, true);
                this._pauseMenu.Close();
                this._pauseGroup.Add((UIComponent)this._pauseMenu, false);
                Options.AddMenus(this._pauseGroup);
                Options.openOnClose = this._pauseMenu;
                this._confirmMenu.Add((UIComponent)new UIMenuItem("NO!", (UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._confirmMenu, (UIComponent)this._pauseMenu), UIAlign.Left, backButton: true), true);
                this._confirmMenu.Add((UIComponent)new UIMenuItem("YES!", (UIMenuAction)new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._quit)), true);
                this._confirmMenu.Close();
                this._pauseGroup.Add((UIComponent)this._confirmMenu, false);
                UIMenu confirmBlacklistMenu1 = this._confirmBlacklistMenu;
                UIText component2 = new UIText("", Color.White, heightAdd: -4f);
                component2.scale = new Vec2(0.5f);
                confirmBlacklistMenu1.Add((UIComponent)component2, true);
                UIMenu confirmBlacklistMenu2 = this._confirmBlacklistMenu;
                UIText component3 = new UIText("Are you sure you want to avoid", Color.White, heightAdd: -4f);
                component3.scale = new Vec2(0.5f);
                confirmBlacklistMenu2.Add((UIComponent)component3, true);
                UIMenu confirmBlacklistMenu3 = this._confirmBlacklistMenu;
                UIText component4 = new UIText("this level in the future?", Color.White, heightAdd: -4f);
                component4.scale = new Vec2(0.5f);
                confirmBlacklistMenu3.Add((UIComponent)component4, true);
                UIMenu confirmBlacklistMenu4 = this._confirmBlacklistMenu;
                UIText component5 = new UIText("", Color.White, heightAdd: -4f);
                component5.scale = new Vec2(0.5f);
                confirmBlacklistMenu4.Add((UIComponent)component5, true);
                this._confirmBlacklistMenu.Add((UIComponent)new UIMenuItem("|DGRED|@blacklist@YES!", (UIMenuAction)new UIMenuActionCloseMenuCallFunction(this._pauseGroup, new UIMenuActionCloseMenuCallFunction.Function(GameMode.Blacklist))), true);
                this._confirmBlacklistMenu.Add((UIComponent)new UIMenuItem("BACK", (UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._confirmBlacklistMenu, (UIComponent)this._pauseMenu), backButton: true), true);
                this._confirmBlacklistMenu.Close();
                this._pauseGroup.Add((UIComponent)this._confirmBlacklistMenu, false);
                Level.Add((Thing)this._pauseGroup);
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
            if ((double)Graphics.fade > 0.899999976158142 && Input.Pressed("START") && !Network.isActive)
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
                    Level.current = (Level)new TeamSelect2(true);
                    this._returnToLobby.value = false;
                }
                if (this._quit.value)
                {
                    if (this._editorTestMode)
                        Level.current = (Level)DuckGameTestArea.currentEditor;
                    else if (this._validityTest)
                        Level.current = (Level)DeathmatchTestDialogue.currentEditor;
                    else if (Network.isActive)
                    {
                        Level.current = (Level)new DisconnectFromGame();
                    }
                    else
                    {
                        Graphics.fade -= 0.04f;
                        if ((double)Graphics.fade >= 0.00999999977648258)
                            return;
                        Level.current = (Level)new TitleScreen();
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
                    if ((double)this._waitFade > 0.0 || !GameMode.getReady || Network.isActive && Network.isClient && !Level.current.transferCompleteCalled)
                        return;
                    this._waitSpawn -= 0.06f;
                    if ((double)this._waitSpawn <= 0.0)
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
                            Level.Add((Thing)new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, 0.0f, new Color((int)color.x, (int)color.y, (int)color.z), 32f));
                            Level.Add((Thing)new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, -4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                            Level.Add((Thing)new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                            Level.Add((Thing)new SpawnAimer(pendingSpawn.x, pendingSpawn.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), pendingSpawn.persona, 4f)
                            {
                                dugg = pendingSpawn
                            });
                            SFX.Play("pullPin", 0.7f);
                            if (pendingSpawn.isServerForObject && !this._editorTestMode)
                            {
                                if (!Network.isActive && pendingSpawn.profile.team.name == "ZEKE")
                                {
                                    Ragdoll ragdoll = new Ragdoll(pendingSpawn.x, pendingSpawn.y, (Duck)null, false, 0.0f, 0, Vec2.Zero);
                                    Level.Add((Thing)ragdoll);
                                    ragdoll.RunInit();
                                    ragdoll.MakeZekeBear();
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Present) || TeamSelect2.Enabled("WINPRES") && GameMode.lastWinners.Contains(pendingSpawn.profile))
                                {
                                    Present h = new Present(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)h);
                                    pendingSpawn.GiveHoldable((Holdable)h);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Jetpack) || TeamSelect2.Enabled("JETTY"))
                                {
                                    Jetpack e = new Jetpack(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)e);
                                    pendingSpawn.Equip((Equipment)e);
                                }
                                if (TeamSelect2.Enabled("HELMY"))
                                {
                                    Helmet e = new Helmet(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)e);
                                    pendingSpawn.Equip((Equipment)e);
                                }
                                if (TeamSelect2.Enabled("SHOESTAR"))
                                {
                                    Boots e = new Boots(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)e);
                                    pendingSpawn.Equip((Equipment)e);
                                }
                                if (DevConsole.fancyMode)
                                {
                                    FancyShoes e = new FancyShoes(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)e);
                                    pendingSpawn.Equip((Equipment)e);
                                }
                                if (TeamSelect2.Enabled("DILLY"))
                                {
                                    DuelingPistol h = new DuelingPistol(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)h);
                                    pendingSpawn.GiveHoldable((Holdable)h);
                                }
                                if (TeamSelect2.Enabled("COOLBOOK"))
                                {
                                    GoodBook h = new GoodBook(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)h);
                                    pendingSpawn.GiveHoldable((Holdable)h);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Armor))
                                {
                                    Helmet e1 = new Helmet(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)e1);
                                    pendingSpawn.Equip((Equipment)e1);
                                    ChestPlate e2 = new ChestPlate(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)e2);
                                    pendingSpawn.Equip((Equipment)e2);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Pistol))
                                {
                                    Pistol h = new Pistol(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)h);
                                    pendingSpawn.GiveHoldable((Holdable)h);
                                }
                                if (Party.HasPerk(pendingSpawn.profile, PartyPerks.NetGun))
                                {
                                    NetGun h = new NetGun(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)h);
                                    pendingSpawn.GiveHoldable((Holdable)h);
                                }
                                if (TeamSelect2.QUACK3)
                                {
                                    Helmet e3 = new Helmet(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)e3);
                                    pendingSpawn.Equip((Equipment)e3);
                                    ChestPlate e4 = new ChestPlate(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)e4);
                                    pendingSpawn.Equip((Equipment)e4);
                                    Holster e5 = new Holster(pendingSpawn.x, pendingSpawn.y);
                                    Level.Add((Thing)e5);
                                    pendingSpawn.Equip((Equipment)e5);
                                    if (pendingSpawn.profile.carryOverObject != null)
                                    {
                                        Level.Add((Thing)pendingSpawn.profile.carryOverObject);
                                        e5.SetContainedObject(pendingSpawn.profile.carryOverObject);
                                    }
                                    else
                                    {
                                        DuelingPistol h = new DuelingPistol(pendingSpawn.x, pendingSpawn.y);
                                        Level.Add((Thing)h);
                                        e5.SetContainedObject((Holdable)h);
                                    }
                                }
                            }
                        }
                        else if (!GameMode._started)
                        {
                            this._waitAfterSpawn -= 0.05f;
                            if ((double)this._waitAfterSpawn <= 0.0)
                            {
                                ++this._waitAfterSpawnDings;
                                if (this._waitAfterSpawnDings > 2)
                                {
                                    Party.Clear();
                                    this.DoStart();
                                    SFX.Play("ding");
                                    Event.Log((Event)new RoundStartEvent());
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
                            if ((double)this._fontFade < 0.0)
                                this._fontFade = 0.0f;
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
                    if ((double)this._roundEndWait < 0.5 && !this._addedPoints && !this.skippedLevel)
                        this.DoAddPoints();
                    if ((double)this._roundEndWait < 0.100000001490116 && !this._endedHighlights)
                    {
                        this._endedHighlights = true;
                        if (!this._editorTestMode)
                            Highlights.FinishRound();
                    }
                    if ((double)this._roundEndWait >= 0.0 || this._switchedLevel)
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
                                Global.WinLevel((Team)null);
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
                                    else if (team.score < 2 && (double)Rando.Float(1f) > 0.300000011920929)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < 5 && (double)Rando.Float(1f) > 0.600000023841858)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < 7 && (double)Rando.Float(1f) > 0.850000023841858)
                                    {
                                        foreach (Profile activeProfile in team.activeProfiles)
                                            Party.AddRandomPerk(activeProfile);
                                    }
                                    else if (team.score < num && (double)Rando.Float(1f) > 0.899999976158142)
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
                                    Send.Message((NetMessage)new NMAssignDraw());
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
                            Send.Message((NetMessage)new NMTransferScores(scrs));
                            GameMode.RunPostRound(this._editorTestMode);
                        }
                    }
                    if (this._validityTest && this._watch != null)
                    {
                        long elapsedMilliseconds = this._watch.ElapsedMilliseconds;
                        if ((double)this.frames / ((double)this._watch.ElapsedMilliseconds / 1000.0) < 30.0)
                        {
                            DeathmatchTestDialogue.success = false;
                            DeathmatchTestDialogue.tooSlow = true;
                        }
                        else
                            DeathmatchTestDialogue.success = true;
                        Level.current = (Level)DeathmatchTestDialogue.currentEditor;
                    }
                    else
                    {
                        if (TeamSelect2.QUACK3)
                        {
                            foreach (Profile profile in Profiles.active)
                                profile.carryOverObject = (Holdable)null;
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
                                        Level.current = (Level)new RockScoreboard(nextLevel, ScoreBoardMode.ShowWinner);
                                        if (!this._editorTestMode)
                                            ++Global.data.drawsPlayed.valueInt;
                                        if (Network.isActive)
                                            Send.Message((NetMessage)new NMDrawBroken());
                                    }
                                    else
                                    {
                                        this._endedHighlights = false;
                                        Level.current = nextLevel;
                                    }
                                }
                                else
                                {
                                    Level.current = (Level)new RockIntro(nextLevel);
                                    this._doScore = false;
                                }
                            }
                            else
                            {
                                this._endedHighlights = false;
                                Level.current = !TeamSelect2.partyMode || this.skippedLevel ? nextLevel : (Level)new DrinkRoom(nextLevel);
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

        protected virtual Level GetNextLevel() => this._editorTestMode ? (Level)new GameLevel((Level.current as GameLevel).levelInputString, editorTestMode: true) : (Level)new GameLevel(Deathmatch.RandomLevelString(GameMode.previousLevel));

        protected void DoAddPoints()
        {
            this._addedPoints = true;
            Event.Log((Event)new RoundEndEvent());
            Highlights.highlightRatingMultiplier = 0.0f;
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
            if (this._waitAfterSpawnDings > 0 && (double)this._fontFade > 0.00999999977648258)
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
            if (!this._validityTest || this._waitAfterSpawnDings <= 0 || (double)this._fontFade >= 0.5)
                return;
            this._pulse += 0.08f;
            string text1 = "WIN THE MATCH";
            float width1 = this._font.GetWidth(text1);
            this._font.alpha = (float)((Math.Sin((double)this._pulse) + 1.0) / 2.0);
            this._font.Draw(text1, (float)((double)Layer.HUD.camera.width / 2.0 - (double)width1 / 2.0), (float)((double)Layer.HUD.camera.height - (double)this._font.height - 16.0), Color.Red);
        }
    }
}
