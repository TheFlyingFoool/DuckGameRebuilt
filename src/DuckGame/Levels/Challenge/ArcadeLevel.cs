// Decompiled with JetBrains decompiler
// Type: DuckGame.ArcadeLevel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class ArcadeLevel : XMLLevel
    {
        protected FollowCam _followCam;
        public Editor editor;
        public string customMachine;
        public LevGenType genType;
        private List<Duck> _pendingSpawns;
        private Duck _duck;
        private ArcadeMode _arcade;
        private UIComponent _pauseGroup;
        private UIMenu _pauseMenu;
        private UIMenu _confirmMenu;
        private UIMenu _advancedMenu;
        private ArcadeState _state;
        private ArcadeState _desiredState;
        private ArcadeHUD _hud;
        private UnlockScreen _unlockScreen;
        private List<ArcadeMachine> _unlockMachines = new List<ArcadeMachine>();
        private bool _unlockingMachine;
        public List<ArcadeMachine> _challenges = new List<ArcadeMachine>();
        private PrizeTable _prizeTable;
        private PlugMachine _plugMachine;
        private object _hoverThing;
        private ArcadeMachine _hoverMachine;
        public static ArcadeLevel currentArcade;
        private bool _launchedChallenge;
        private bool _flipState;
        private float _unlockMachineWait = 1f;
        private bool _paused;
        private bool _quitting;
        private MenuBoolean _quit = new MenuBoolean();
        private bool _afterChallenge;
        private List<ArcadeFrame> _frames = new List<ArcadeFrame>();
        public bool basementWasUnlocked;
        private MetroidDoor _exitDoor;
        private UIDivider _pausebox;
        private bool _entering = true;
        private bool _enteringCameraUpdated;
        private bool spawnKey;
        private float spawnKeyWait = 0.2f;
        private bool _prevGotDev;
        public bool returnToChallengeList;
        public bool launchSpecialChallenge;
        private Sprite _speedClock;

        public FollowCam followCam => this._followCam;

        public ArcadeLevel(string name)
          : base(name)
        {
            this._followCam = new FollowCam
            {
                lerpMult = 2f,
                startCentered = false
            };
            this.camera = _followCam;
        }

        public void UpdateDefault()
        {
            if (Level.current == null)
                return;
            foreach (Door door in Level.current.things[typeof(Door)])
            {
                if (door._lockDoor)
                    door.locked = !Unlocks.IsUnlocked("BASEMENTKEY", Profiles.active[0]);
            }
        }

        public void CheckFrames()
        {
            float challengeSkillIndex = Challenges.GetChallengeSkillIndex();
            foreach (ArcadeFrame frame in this._frames)
            {
                if ((double)challengeSkillIndex >= (double)(float)frame.respect && ChallengeData.CheckRequirement(Profiles.active[0], (string)frame.requirement))
                    frame.visible = true;
                else
                    frame.visible = false;
            }
        }

        public ArcadeFrame GetFrame()
        {
            float challengeSkillIndex = Challenges.GetChallengeSkillIndex();
            foreach (ArcadeFrame frame in (IEnumerable<ArcadeFrame>)this._frames.OrderBy<ArcadeFrame, int>(x => x.saveData == null ? Rando.Int(100) : Rando.Int(100) + 200))
            {
                if ((double)challengeSkillIndex >= (double)(float)frame.respect && ChallengeData.CheckRequirement(Profiles.active[0], (string)frame.requirement))
                    return frame;
            }
            return null;
        }

        public ArcadeFrame GetFrame(string id) => this._frames.FirstOrDefault<ArcadeFrame>(x => x._identifier == id);

        public void InitializeMachines()
        {
            base.Initialize();
            foreach (ArcadeMachine arcadeMachine in this.things[typeof(ArcadeMachine)])
                this._challenges.Add(arcadeMachine);
        }

        public override void Initialize()
        {
            TeamSelect2.DefaultSettings();
            base.Initialize();
            this._pendingSpawns = new Deathmatch(this).SpawnPlayers(false);
            foreach (Duck pendingSpawn in this._pendingSpawns)
            {
                this.followCam.Add(pendingSpawn);
                Level.First<ArcadeHatConsole>()?.MakeHatSelector(pendingSpawn);
            }
            this.UpdateDefault();
            this.followCam.Adjust();
            if (this.genType == LevGenType.CustomArcadeMachine)
            {
                if (this.things[typeof(ArcadeMachine)].FirstOrDefault<Thing>() is ArcadeMachine arcadeMachine1)
                {
                    LevelData levelData = DuckFile.LoadLevel(this.customMachine);
                    if (levelData != null && levelData.objects != null)
                    {
                        if (levelData.objects.objects != null)
                        {
                            try
                            {
                                if (Thing.LoadThing(levelData.objects.objects.FirstOrDefault<BinaryClassChunk>()) is ImportMachine importMachine)
                                {
                                    importMachine.position = arcadeMachine1.position;
                                    Level.Remove(arcadeMachine1);
                                    Level.Add(importMachine);
                                    this.things.RefreshState();
                                    this._challenges.Add(importMachine);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            else
            {
                int num = this.bareInitialize ? 1 : 0;
                foreach (ArcadeMachine arcadeMachine2 in this.things[typeof(ArcadeMachine)])
                    this._challenges.Add(arcadeMachine2);
            }
            Profiles.active[0].ticketCount = Challenges.GetTicketCount(Profiles.active[0]);
            foreach (ArcadeFrame arcadeFrame in this.things[typeof(ArcadeFrame)])
                this._frames.Add(arcadeFrame);
            foreach (ChallengeSaveData challengeSaveData in Challenges.GetAllSaveData())
            {
                if (challengeSaveData.frameID != "")
                {
                    ArcadeFrame frame = this.GetFrame(challengeSaveData.frameID);
                    if (frame != null)
                        frame.saveData = challengeSaveData;
                }
            }
            foreach (ArcadeMachine challenge in this._challenges)
                challenge.unlocked = challenge.CheckUnlocked(false);
            this._hud = new ArcadeHUD
            {
                alpha = 0f
            };
            Level.Add(_hud);
            this._unlockScreen = new UnlockScreen
            {
                alpha = 0f
            };
            Level.Add(_unlockScreen);
            this._pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            this._pauseMenu = new UIMenu("@LWING@ARCADE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE  @SELECT@SELECT");
            this._confirmMenu = new UIMenu("EXIT ARCADE?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            this._advancedMenu = new UIMenu("@LWING@ADVANCED@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 260f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            this._pausebox = new UIDivider(true, 0.8f);
            this._pausebox.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(this._pauseGroup), UIAlign.Left), true);
            this._pausebox.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
            this._pausebox.leftSection.Add(new UIText("", Color.White), true);
            this._pausebox.leftSection.Add(new UIMenuItem("|DGRED|EXIT ARCADE", new UIMenuActionOpenMenu(_pauseMenu, _confirmMenu), UIAlign.Left), true);
            this._pausebox.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
            this._pauseMenu.Add(_pausebox, true);
            this._pauseMenu.Close();
            this._pauseGroup.Add(_pauseMenu, false);
            Options.AddMenus(this._pauseGroup);
            Options.openOnClose = this._pauseMenu;
            this._confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_confirmMenu, _pauseMenu), UIAlign.Left, backButton: true), true);
            this._confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(this._pauseGroup, this._quit)), true);
            this._confirmMenu.Close();
            this._pauseGroup.Add(_confirmMenu, false);
            this._advancedMenu.Add(new UIText("|DGBLUE|SPEEDRUN SETTINGS", Color.White), true);
            this._advancedMenu.Add(new UIText("", Color.White), true);
            this._advancedMenu.Add(new UIText("If enabled, Speedrun Mode", Colors.DGBlue), true);
            this._advancedMenu.Add(new UIText("will fix the random generator", Colors.DGBlue), true);
            this._advancedMenu.Add(new UIText("to make target spawns", Colors.DGBlue), true);
            this._advancedMenu.Add(new UIText("deterministic.", Colors.DGBlue), true);
            this._advancedMenu.Add(new UIMenuItemToggle("SPEEDRUN MODE", field: new FieldBinding(DuckNetwork.core, "speedrunMode")), true);
            this._advancedMenu.Add(new UIMenuItemToggle("MAX TROPHY", field: new FieldBinding(DuckNetwork.core, "speedrunMaxTrophy", max: 5f), multi: new List<string>()
      {
        "OFF",
        "@BRONZE@",
        "@SILVER@",
        "@GOLD@",
        "@PLATINUM@",
        "@DEVELOPER@"
      }, compressedMulti: true), true);
            this._advancedMenu.Add(new UIText("", Color.White), true);
            this._advancedMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_advancedMenu, _pauseMenu), backButton: true), true);
            this._advancedMenu.Close();
            this._pauseGroup.Add(_advancedMenu, false);
            this._pauseGroup.isPauseMenu = true;
            this._pauseGroup.Close();
            Level.Add(_pauseGroup);
            this._prizeTable = this.things[typeof(PrizeTable)].FirstOrDefault<Thing>() as PrizeTable;
            this._plugMachine = this.things[typeof(PlugMachine)].FirstOrDefault<Thing>() as PlugMachine;
            if (this._prizeTable == null)
                this._prizeTable = new PrizeTable(730f, 124f);
            Chancy.activeChallenge = null;
            Chancy.atCounter = true;
            Chancy.lookingAtChallenge = false;
            this.basementWasUnlocked = Unlocks.IsUnlocked("BASEMENTKEY", Profiles.active[0]);
            Level.Add(_prizeTable);
            Music.Play("Arcade");
            this._exitDoor = new MetroidDoor(-192f, 320f);
            Level.Add(_exitDoor);
            this._followCam.hardLimitLeft = -192f;
        }

        public override void Terminate()
        {
        }

        public override void Update()
        {
            ++MonoMain.timeInArcade;
            if (!this._prevGotDev && Options.Data.gotDevMedal)
            {
                this._prevGotDev = true;
                this._pausebox.leftSection.Insert(new UIMenuItem("ADVANCED", new UIMenuActionOpenMenu(_pauseMenu, _advancedMenu), UIAlign.Left), 2, true);
            }
            if (this._entering)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                if (Graphics.fade > 0.99f)
                {
                    this._entering = false;
                    Graphics.fade = 1f;
                }
            }
            Options.openOnClose = this._pauseMenu;
            if (this._duck != null && this._duck.profile != null && !this._duck.profile.inputProfile.HasAnyConnectedDevice())
            {
                foreach (InputProfile defaultProfile in InputProfile.defaultProfiles)
                {
                    if (defaultProfile.HasAnyConnectedDevice())
                    {
                        InputProfile.SwapDefaultInputStrings(defaultProfile.name, this._duck.profile.inputProfile.name);
                        InputProfile.ReassignDefaultInputProfiles();
                        this._duck.profile.inputProfile = defaultProfile;
                        break;
                    }
                }
            }
            if (this.spawnKey)
            {
                if (spawnKeyWait > 0.0)
                {
                    this.spawnKeyWait -= Maths.IncFrameTimer();
                }
                else
                {
                    SFX.Play("ching");
                    this.spawnKey = false;
                    Key key = new Key(this._prizeTable.x, this._prizeTable.y)
                    {
                        vSpeed = -4f,
                        depth = this._duck.depth + 50
                    };
                    Level.Add(SmallSmoke.New(key.x + Rando.Float(-4f, 4f), key.y + Rando.Float(-4f, 4f)));
                    Level.Add(SmallSmoke.New(key.x + Rando.Float(-4f, 4f), key.y + Rando.Float(-4f, 4f)));
                    Level.Add(SmallSmoke.New(key.x + Rando.Float(-4f, 4f), key.y + Rando.Float(-4f, 4f)));
                    Level.Add(SmallSmoke.New(key.x + Rando.Float(-4f, 4f), key.y + Rando.Float(-4f, 4f)));
                    Level.Add(key);
                }
            }
            Chancy.Update();
            if (this._pendingSpawns != null && this._pendingSpawns.Count > 0)
            {
                Duck pendingSpawn = this._pendingSpawns[0];
                this.AddThing(pendingSpawn);
                this._pendingSpawns.RemoveAt(0);
                this._duck = pendingSpawn;
                this._arcade = this.things[typeof(ArcadeMode)].First<Thing>() as ArcadeMode;
                if (!this._enteringCameraUpdated)
                {
                    this._enteringCameraUpdated = true;
                    for (int index = 0; index < 200; ++index)
                        this._followCam.Update();
                }
            }
            double num = (double)Math.Min(1f, Math.Max(0f, (float)((1.0 - (double)Layer.Game.fade) * 1.5)));
            this.backgroundColor = Color.Black;
            if (UnlockScreen.open || ArcadeHUD.open)
            {
                foreach (Thing challenge in this._challenges)
                    challenge.visible = false;
                this._prizeTable.visible = false;
            }
            else
            {
                foreach (Thing challenge in this._challenges)
                    challenge.visible = true;
                this._prizeTable.visible = true;
            }
            if (this._duck != null)
                this._exitDoor._arcadeProfile = this._duck.profile;
            if (this._state == this._desiredState && this._state != ArcadeState.UnlockMachine && this._state != ArcadeState.LaunchChallenge)
            {
                if (!this._quitting)
                {
                    ArcadeHatConsole arcadeHatConsole = Level.First<ArcadeHatConsole>();
                    if (Input.Pressed("START") && (arcadeHatConsole == null || !arcadeHatConsole.IsOpen()))
                    {
                        this._pauseGroup.Open();
                        this._pauseMenu.Open();
                        MonoMain.pauseMenu = this._pauseGroup;
                        if (!this._paused)
                        {
                            Music.Pause();
                            SFX.Play("pause", 0.6f);
                            this._paused = true;
                            this._duck.immobilized = true;
                        }
                        this.simulatePhysics = false;
                        return;
                    }
                    if (this._paused && MonoMain.pauseMenu == null)
                    {
                        this._paused = false;
                        SFX.Play("resume", 0.6f);
                        if (this._quit.value)
                        {
                            this._quitting = true;
                        }
                        else
                        {
                            Music.Resume();
                            this._duck.immobilized = false;
                            this.simulatePhysics = true;
                        }
                    }
                }
                else
                {
                    Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.05f);
                    if (Graphics.fade > 0.01f)
                        return;
                    Chancy.StopShowingChallengeList();
                    if (this.editor != null)
                    {
                        Level.current = editor;
                        return;
                    }
                    Level.current = new TitleScreen();
                    return;
                }
            }
            if (this._paused)
                return;
            if (this._hud.launchChallenge)
                this._desiredState = ArcadeState.LaunchChallenge;
            if (this._desiredState != this._state)
            {
                this._duck.active = false;
                bool flag = false;
                if (this._desiredState == ArcadeState.ViewChallenge)
                {
                    this._duck.alpha = Lerp.FloatSmooth(this._duck.alpha, 0f, 0.1f);
                    this._followCam.manualViewSize = Lerp.FloatSmooth(this._followCam.manualViewSize, 2f, 0.16f);
                    if (_followCam.manualViewSize < 30.0)
                    {
                        Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        this._hud.alpha = Lerp.Float(this._hud.alpha, 1f, 0.08f);
                        if (_followCam.manualViewSize < 3.0 && (double)this._hud.alpha == 1.0 && (double)Layer.Game.fade == 0.0)
                            flag = true;
                    }
                }
                else if (this._desiredState == ArcadeState.Normal)
                {
                    if (!this._flipState)
                    {
                        this._followCam.Clear();
                        this._followCam.Add(_duck);
                        HUD.CloseAllCorners();
                    }
                    this._duck.alpha = Lerp.FloatSmooth(this._duck.alpha, 1f, 0.1f, 1.1f);
                    if (this._state == ArcadeState.ViewChallenge || this._state == ArcadeState.UnlockScreen)
                        this._followCam.manualViewSize = Lerp.FloatSmooth(this._followCam.manualViewSize, this._followCam.viewSize, 0.14f, 1.05f);
                    Layer.Game.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    Layer.Background.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    this._hud.alpha = Lerp.Float(this._hud.alpha, 0f, 0.08f);
                    this._unlockScreen.alpha = Lerp.Float(this._unlockScreen.alpha, 0f, 0.08f);
                    if ((_followCam.manualViewSize < 0.0 || _followCam.manualViewSize == (double)this._followCam.viewSize) && (double)this._hud.alpha == 0.0 && (double)Layer.Game.fade == 1.0)
                    {
                        flag = true;
                        this._followCam.manualViewSize = -1f;
                        this._duck.alpha = 1f;
                    }
                    if (Unlockables.HasPendingUnlocks())
                        MonoMain.pauseMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList<Unlockable>(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                }
                else if (this._desiredState == ArcadeState.ViewSpecialChallenge || this._desiredState == ArcadeState.ViewChallengeList || this._desiredState == ArcadeState.ViewProfileSelector)
                {
                    if (!this._flipState)
                    {
                        this._followCam.Clear();
                        this._followCam.Add(_duck);
                        HUD.CloseAllCorners();
                    }
                    this._duck.alpha = Lerp.FloatSmooth(this._duck.alpha, 1f, 0.1f, 1.1f);
                    if (this._state == ArcadeState.ViewChallenge || this._state == ArcadeState.UnlockScreen)
                        this._followCam.manualViewSize = Lerp.FloatSmooth(this._followCam.manualViewSize, this._followCam.viewSize, 0.14f, 1.05f);
                    Layer.Game.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    Layer.Background.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    this._hud.alpha = Lerp.Float(this._hud.alpha, 0f, 0.08f);
                    this._unlockScreen.alpha = Lerp.Float(this._unlockScreen.alpha, 0f, 0.08f);
                    if ((_followCam.manualViewSize < 0.0 || _followCam.manualViewSize == (double)this._followCam.viewSize) && (double)this._hud.alpha == 0.0 && (double)Layer.Game.fade == 1.0)
                    {
                        flag = true;
                        this._followCam.manualViewSize = -1f;
                        this._duck.alpha = 1f;
                    }
                }
                else if (this._desiredState == ArcadeState.UnlockMachine)
                {
                    if (!this._flipState)
                    {
                        this._followCam.Clear();
                        this._followCam.Add(this._unlockMachines[0]);
                        HUD.CloseAllCorners();
                    }
                    if (this._state == ArcadeState.ViewChallenge)
                        this._followCam.manualViewSize = Lerp.FloatSmooth(this._followCam.manualViewSize, this._followCam.viewSize, 0.14f, 1.05f);
                    this._duck.alpha = Lerp.FloatSmooth(this._duck.alpha, 1f, 0.1f, 1.1f);
                    Layer.Game.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    Layer.Background.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    this._hud.alpha = Lerp.Float(this._hud.alpha, 0f, 0.08f);
                    this._unlockScreen.alpha = Lerp.Float(this._unlockScreen.alpha, 0f, 0.08f);
                    this._unlockMachineWait = 1f;
                    if ((_followCam.manualViewSize < 0.0 || _followCam.manualViewSize == (double)this._followCam.viewSize) && (double)this._hud.alpha == 0.0 && (double)Layer.Game.fade == 1.0)
                    {
                        flag = true;
                        this._followCam.manualViewSize = -1f;
                        this._duck.alpha = 1f;
                    }
                }
                else if (this._desiredState == ArcadeState.LaunchChallenge)
                {
                    if (!this._flipState)
                        HUD.CloseAllCorners();
                    Music.volume = Lerp.Float(Music.volume, 0f, 0.01f);
                    this._hud.alpha = Lerp.Float(this._hud.alpha, 0f, 0.02f);
                    this._unlockScreen.alpha = Lerp.Float(this._unlockScreen.alpha, 0f, 0.08f);
                    if ((double)this._hud.alpha == 0.0)
                        flag = true;
                }
                if (this._desiredState == ArcadeState.UnlockScreen)
                {
                    this._duck.alpha = Lerp.FloatSmooth(this._duck.alpha, 0f, 0.1f);
                    this._followCam.manualViewSize = Lerp.FloatSmooth(this._followCam.manualViewSize, 2f, 0.16f);
                    if (_followCam.manualViewSize < 30.0)
                    {
                        Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        this._unlockScreen.alpha = Lerp.Float(this._unlockScreen.alpha, 1f, 0.08f);
                        if (_followCam.manualViewSize < 3.0 && (double)this._unlockScreen.alpha == 1.0 && (double)Layer.Game.fade == 0.0)
                            flag = true;
                    }
                }
                if (this._desiredState == ArcadeState.Plug)
                    flag = true;
                this._flipState = true;
                if (this._launchedChallenge)
                {
                    Layer.Background.fade = 0f;
                    Layer.Game.fade = 0f;
                }
                if (flag)
                {
                    this._flipState = false;
                    HUD.CloseAllCorners();
                    this._state = this._desiredState;
                    if (this._state == ArcadeState.ViewChallenge)
                    {
                        if (this._afterChallenge)
                        {
                            Music.Play("Arcade");
                            this._afterChallenge = false;
                        }
                        this._hud.MakeActive();
                        Level.Add(_hud);
                        this._duck.active = false;
                    }
                    else
                    {
                        if (this._state == ArcadeState.LaunchChallenge)
                        {
                            ArcadeLevel.currentArcade = this;
                            foreach (Thing thing in this.things[typeof(ChallengeConfetti)])
                                Level.Remove(thing);
                            Music.Stop();
                            Level.current = new ChallengeLevel(this._hud.selected.challenge.fileName);
                            if (!this.launchSpecialChallenge)
                            {
                                this._desiredState = ArcadeState.ViewChallenge;
                                this._hud.launchChallenge = false;
                                this._launchedChallenge = false;
                                this._afterChallenge = true;
                                return;
                            }
                            this._desiredState = ArcadeState.ViewSpecialChallenge;
                            this._hud.launchChallenge = false;
                            this._launchedChallenge = false;
                            this._afterChallenge = true;
                            this.launchSpecialChallenge = false;
                            return;
                        }
                        if (this._state != ArcadeState.UnlockMachine)
                        {
                            if (this._state == ArcadeState.Normal)
                            {
                                this._unlockMachines.Clear();
                                foreach (ArcadeMachine challenge in this._challenges)
                                {
                                    if (challenge.CheckUnlocked())
                                        this._unlockMachines.Add(challenge);
                                }
                                if (this._unlockMachines.Count > 0)
                                {
                                    this._desiredState = ArcadeState.UnlockMachine;
                                    return;
                                }
                                if (!this.basementWasUnlocked && Unlocks.IsUnlocked("BASEMENTKEY", Profiles.active[0]))
                                {
                                    this.spawnKey = true;
                                    this.basementWasUnlocked = true;
                                }
                                this._duck.active = true;
                            }
                            else if (this._state == ArcadeState.ViewSpecialChallenge)
                            {
                                this._duck.active = false;
                                if (this._afterChallenge)
                                {
                                    Music.Play("Arcade");
                                    this._afterChallenge = false;
                                    HUD.AddCornerCounter(HUDCorner.BottomMiddle, "@TICKET@ ", new FieldBinding(Profiles.active[0], "ticketCount"), animateCount: true);
                                    Chancy.afterChallenge = true;
                                    Chancy.afterChallengeWait = 1f;
                                }
                                else
                                {
                                    HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@ACCEPT");
                                    HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@CANCEL");
                                    HUD.AddCornerCounter(HUDCorner.BottomMiddle, "@TICKET@ ", new FieldBinding(Profiles.active[0], "ticketCount"), animateCount: true);
                                }
                                this._duck.active = false;
                            }
                            else if (this._state == ArcadeState.ViewProfileSelector)
                            {
                                this._duck.active = false;
                                ArcadeHatConsole arcadeHatConsole = Level.First<ArcadeHatConsole>();
                                if (arcadeHatConsole != null)
                                {
                                    HUD.CloseAllCorners();
                                    arcadeHatConsole.Open();
                                }
                            }
                            else if (this._state == ArcadeState.ViewChallengeList)
                            {
                                this._duck.active = false;
                                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@ACCEPT");
                                HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
                            }
                            else if (this._state == ArcadeState.UnlockScreen)
                            {
                                this.basementWasUnlocked = Unlocks.IsUnlocked("BASEMENTKEY", Profiles.active[0]);
                                this._unlockScreen.MakeActive();
                                this._duck.active = false;
                            }
                            else if (this._state == ArcadeState.Plug)
                            {
                                Plug.Add("Yo! I'm |DGBLUE|[QC] WUMP|WHITE|, Chancy's Sister.^Machine behind me plays |DGYELLOW|user challenges|WHITE|, as well^as obscenely hard challenges I made myself!");
                                Plug.Add("And the machine behind this one is today's^|DGYELLOW|Imported Machine.");
                                Plug.Open();
                                this._duck.active = false;
                            }
                        }
                    }
                }
            }
            else if (this._state == ArcadeState.Normal || this._state == ArcadeState.UnlockMachine)
            {
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, 1f, 0.08f);
                Layer.Background.fade = Lerp.Float(Layer.Game.fade, 1f, 0.08f);
                this._hud.alpha = Lerp.Float(this._hud.alpha, 0f, 0.08f);
                if (this._state == ArcadeState.Normal)
                {
                    object obj = null;
                    foreach (ArcadeMachine challenge in this._challenges)
                    {
                        double length = (double)(this._duck.position - challenge.position).length;
                        if (challenge.hover)
                        {
                            obj = challenge;
                            if (Input.Pressed("SHOOT"))
                            {
                                this._hud.activeChallengeGroup = challenge.data;
                                this._desiredState = ArcadeState.ViewChallenge;
                                this._followCam.manualViewSize = this._followCam.viewSize;
                                this._followCam.Clear();
                                this._followCam.Add(challenge);
                                HUD.CloseAllCorners();
                                this._hoverMachine = null;
                                this._hoverThing = null;
                                return;
                            }
                        }
                        if (this._prizeTable.hover)
                        {
                            obj = _prizeTable;
                            if (Input.Pressed("SHOOT"))
                            {
                                this._desiredState = ArcadeState.UnlockScreen;
                                this._followCam.manualViewSize = this._followCam.viewSize;
                                this._followCam.Clear();
                                this._followCam.Add(_prizeTable);
                                HUD.CloseAllCorners();
                                this._hoverMachine = null;
                                this._hoverThing = null;
                                return;
                            }
                        }
                        else if (this._plugMachine != null && this._plugMachine.hover)
                        {
                            obj = _plugMachine;
                            if (Input.Pressed("SHOOT"))
                            {
                                this._desiredState = ArcadeState.Plug;
                                this._followCam.manualViewSize = this._followCam.viewSize;
                                this._followCam.Clear();
                                this._followCam.Add(_plugMachine);
                                HUD.CloseAllCorners();
                                this._hoverMachine = null;
                                this._hoverThing = null;
                                return;
                            }
                        }
                    }
                    if (Chancy.hover && Input.Pressed("SHOOT"))
                    {
                        this._desiredState = ArcadeState.ViewSpecialChallenge;
                        HUD.CloseAllCorners();
                        this._hoverMachine = null;
                        this._hoverThing = null;
                        Chancy.hover = false;
                        Chancy.lookingAtChallenge = true;
                        Chancy.OpenChallengeView();
                        return;
                    }
                    ArcadeHatConsole arcadeHatConsole = Level.First<ArcadeHatConsole>();
                    if (arcadeHatConsole != null && Input.Pressed("SHOOT") && arcadeHatConsole.hover)
                    {
                        this._desiredState = ArcadeState.ViewProfileSelector;
                        HUD.CloseAllCorners();
                        this._hoverMachine = null;
                        this._hoverThing = null;
                        return;
                    }
                    Chancy.hover = false;
                    if (!Chancy.atCounter)
                    {
                        if ((double)(this._duck.position - Chancy.standingPosition).length < 22.0)
                        {
                            obj = Chancy.context;
                            Chancy.hover = true;
                        }
                        if (Chancy.standingPosition.x < (double)Layer.Game.camera.left - 16.0 || Chancy.standingPosition.x > (double)Layer.Game.camera.right + 16.0 || Chancy.standingPosition.y < (double)Layer.Game.camera.top - 16.0 || Chancy.standingPosition.y > (double)Layer.Game.camera.bottom + 16.0)
                        {
                            Chancy.atCounter = true;
                            Chancy.activeChallenge = null;
                        }
                    }
                    else if (this._prizeTable.hoverChancyChallenge)
                    {
                        obj = _arcade;
                        if (Input.Pressed("SHOOT"))
                        {
                            this._desiredState = ArcadeState.ViewChallengeList;
                            HUD.CloseAllCorners();
                            Chancy.OpenChallengeList();
                            this._hoverMachine = null;
                            this._hoverThing = null;
                            Chancy.hover = false;
                            Chancy.lookingAtList = true;
                            return;
                        }
                    }
                    if (this._hoverThing != obj)
                    {
                        HUD.CloseAllCorners();
                        this._hoverThing = obj;
                        this._hoverMachine = !(this._hoverThing is ArcadeMachine) ? null : obj as ArcadeMachine;
                        if (this._hoverMachine != null)
                        {
                            HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@PLAY");
                            string text = this._hoverMachine.data.GetNameForDisplay() + " ";
                            foreach (string challenge1 in this._hoverMachine.data.challenges)
                            {
                                ChallengeData challenge2 = Challenges.GetChallenge(challenge1);
                                if (challenge2 != null)
                                {
                                    ChallengeSaveData saveData = this._duck.profile.GetSaveData(challenge2.levelID);
                                    if (saveData.trophy == TrophyType.Baseline)
                                        text += "@BASELINE@";
                                    else if (saveData.trophy == TrophyType.Bronze)
                                        text += "@BRONZE@";
                                    else if (saveData.trophy == TrophyType.Silver)
                                        text += "@SILVER@";
                                    else if (saveData.trophy == TrophyType.Gold)
                                        text += "@GOLD@";
                                    else if (saveData.trophy == TrophyType.Platinum)
                                        text += "@PLATINUM@";
                                    else if (saveData.trophy == TrophyType.Developer)
                                        text += "@DEVELOPER@";
                                }
                            }
                            HUD.AddCornerMessage(HUDCorner.TopRight, text);
                        }
                        else if (this._prizeTable.hover)
                        {
                            if (this._prizeTable.hoverChancyChallenge)
                            {
                                HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@VIEW CHALLENGES");
                            }
                            else
                            {
                                HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@CHANCY");
                                HUD.AddCornerCounter(HUDCorner.BottomMiddle, "@TICKET@ ", new FieldBinding(Profiles.active[0], "ticketCount"), animateCount: true);
                            }
                        }
                        else
                        {
                            switch (obj)
                            {
                                case ArcadeMode _:
                                    if (this._prizeTable.hoverChancyChallenge)
                                    {
                                        HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@VIEW CHALLENGES");
                                        break;
                                    }
                                    break;
                                case Chancy _:
                                    HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@CHANCY");
                                    break;
                                case PlugMachine _:
                                    HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@[QC]WUMP");
                                    break;
                            }
                        }
                    }
                }
                else if (this._state == ArcadeState.UnlockMachine)
                {
                    this._unlockMachineWait -= 0.02f;
                    if (_unlockMachineWait < 0.0)
                    {
                        if (this._unlockingMachine)
                        {
                            this._unlockingMachine = false;
                            this._followCam.Clear();
                            this._followCam.Add(this._unlockMachines[0]);
                            this._unlockMachineWait = 1f;
                        }
                        else if (this._unlockMachines.Count > 0)
                        {
                            this._unlockMachines[0].unlocked = true;
                            this._unlockMachines.RemoveAt(0);
                            this._unlockingMachine = this._unlockMachines.Count > 0;
                            SFX.Play("lightTurnOn", pitch: Rando.Float(-0.1f, 0.1f));
                            this._unlockMachineWait = 1f;
                        }
                        else
                            this._desiredState = ArcadeState.Normal;
                    }
                }
            }
            else if (this._state == ArcadeState.ViewChallenge)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0f, 0.05f);
                Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0f, 0.05f);
                this._hud.alpha = Lerp.Float(this._hud.alpha, 1f, 0.05f);
                if (this._hud.quitOut)
                {
                    this._hud.quitOut = false;
                    this._desiredState = ArcadeState.Normal;
                    if (Chancy.activeChallenge == null)
                    {
                        List<ChallengeData> chancyChallenges = Challenges.GetEligibleIncompleteChancyChallenges(Profiles.active[0]);
                        if (chancyChallenges.Count > 0)
                        {
                            Vec2 position = this._duck.position;
                            ArcadeMachine arcadeMachine = Level.Nearest<ArcadeMachine>(this._duck.x, this._duck.y);
                            if (arcadeMachine != null)
                                position = arcadeMachine.position;
                            chancyChallenges.OrderBy<ChallengeData, int>(v => v.GetRequirementValue());
                            Chancy.AddProposition(chancyChallenges[chancyChallenges.Count - 1], position);
                        }
                    }
                }
            }
            else if (this._state == ArcadeState.UnlockScreen)
            {
                if (this._unlockScreen.quitOut)
                {
                    this._unlockScreen.quitOut = false;
                    this._desiredState = ArcadeState.Normal;
                }
            }
            else if (this._state == ArcadeState.ViewSpecialChallenge)
            {
                if (!this.launchSpecialChallenge)
                {
                    Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                    if (Input.Pressed("CANCEL"))
                    {
                        if (this.returnToChallengeList)
                        {
                            this._desiredState = ArcadeState.ViewChallengeList;
                            Chancy.hover = false;
                            Chancy.lookingAtList = true;
                        }
                        else
                            this._desiredState = ArcadeState.Normal;
                        Chancy.lookingAtChallenge = false;
                        HUD.CloseAllCorners();
                        SFX.Play("consoleCancel");
                        return;
                    }
                    if (Input.Pressed("SELECT"))
                    {
                        this.launchSpecialChallenge = true;
                        SFX.Play("consoleSelect");
                        return;
                    }
                }
                else
                {
                    Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.05f);
                    if ((double)Graphics.fade < 0.01f)
                    {
                        this._hud.launchChallenge = true;
                        this._hud.selected = new ChallengeCard(0f, 0f, Chancy.activeChallenge);
                        HUD.CloseAllCorners();
                    }
                }
            }
            else if (this._state == ArcadeState.ViewChallengeList)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                if (Input.Pressed("CANCEL"))
                {
                    this._desiredState = ArcadeState.Normal;
                    Chancy.lookingAtChallenge = false;
                    Chancy.lookingAtList = false;
                    HUD.CloseAllCorners();
                    SFX.Play("consoleCancel");
                    return;
                }
                if (Input.Pressed("SELECT"))
                {
                    Chancy.AddProposition(Chancy.selectedChallenge, Chancy.standingPosition);
                    this.returnToChallengeList = true;
                    this._desiredState = ArcadeState.ViewSpecialChallenge;
                    HUD.CloseAllCorners();
                    this._hoverMachine = null;
                    this._hoverThing = null;
                    Chancy.hover = false;
                    Chancy.lookingAtChallenge = true;
                    Chancy.lookingAtList = false;
                    Chancy.OpenChallengeView();
                }
            }
            else if (this._state == ArcadeState.ViewProfileSelector)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                ArcadeHatConsole arcadeHatConsole = Level.First<ArcadeHatConsole>();
                if (arcadeHatConsole != null && !arcadeHatConsole.IsOpen())
                {
                    foreach (ArcadeMachine challenge in this._challenges)
                        challenge.unlocked = challenge.CheckUnlocked(false);
                    this._unlockMachines.Clear();
                    this.UpdateDefault();
                    this._desiredState = ArcadeState.Normal;
                }
            }
            if (!Plug.open)
                return;
            Plug.Update();
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD)
            {
                Chancy.Draw();
                if (Plug.open)
                    Plug.Draw();
                if (this._speedClock == null)
                    this._speedClock = new Sprite("speedrunClock");
                if (DuckNetwork.core.speedrunMode)
                    Graphics.Draw(this._speedClock, 4f, 4f);
            }
            if (layer == Layer.Game)
                Chancy.DrawGameLayer();
            base.PostDrawLayer(layer);
        }
    }
}
