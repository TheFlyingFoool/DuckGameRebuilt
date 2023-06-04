// Decompiled with JetBrains decompiler
// Type: DuckGame.ArcadeLevel
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public FollowCam followCam => _followCam;
        public bool sign;
        public ArcadeLevel(string name)
          : base(name)
        {
            _followCam = new FollowCam
            {
                lerpMult = 2f,
                startCentered = false
            };
            camera = _followCam;
        }

        public void UpdateDefault()
        {
            if (current == null)
                return;
            foreach (Door door in current.things[typeof(Door)])
            {
                if (door._lockDoor)
                    door.locked = !Unlocks.IsUnlocked("BASEMENTKEY", Profiles.active[0]);
            }
        }

        public void CheckFrames()
        {
            float challengeSkillIndex = Challenges.GetChallengeSkillIndex();
            foreach (ArcadeFrame frame in _frames)
            {
                if (challengeSkillIndex >= (float)frame.respect && ChallengeData.CheckRequirement(Profiles.active[0], (string)frame.requirement))
                    frame.visible = true;
                else
                    frame.visible = false;
            }
        }

        public ArcadeFrame GetFrame()
        {
            float challengeSkillIndex = Challenges.GetChallengeSkillIndex();
            foreach (ArcadeFrame frame in (IEnumerable<ArcadeFrame>)_frames.OrderBy(x => x.saveData == null ? Rando.Int(100) : Rando.Int(100) + 200))
            {
                if (challengeSkillIndex >= (float)frame.respect && ChallengeData.CheckRequirement(Profiles.active[0], (string)frame.requirement))
                    return frame;
            }
            return null;
        }

        public ArcadeFrame GetFrame(string id) => _frames.FirstOrDefault(x => x._identifier == id);

        public void InitializeMachines()
        {
            base.Initialize();
            foreach (ArcadeMachine arcadeMachine in things[typeof(ArcadeMachine)])
                _challenges.Add(arcadeMachine);
        }

        public override void Initialize()
        {
            if (sign)
            {
                Add(new VersionSign(-165, 256) { fadeTime = 300 });
            }
            TeamSelect2.DefaultSettings();
            base.Initialize();
            _pendingSpawns = new Deathmatch(this).SpawnPlayers(false);
            foreach (Duck pendingSpawn in _pendingSpawns)
            {
                SpawnPosition = pendingSpawn.position;
                followCam.Add(pendingSpawn);
                First<ArcadeHatConsole>()?.MakeHatSelector(pendingSpawn);
            }
            UpdateDefault();
            followCam.Adjust();
            if (genType == LevGenType.CustomArcadeMachine)
            {
                if (things[typeof(ArcadeMachine)].FirstOrDefault() is ArcadeMachine arcadeMachine1)
                {
                    LevelData levelData = DuckFile.LoadLevel(customMachine);
                    if (levelData != null && levelData.objects != null)
                    {
                        if (levelData.objects.objects != null)
                        {
                            try
                            {
                                if (Thing.LoadThing(levelData.objects.objects.FirstOrDefault()) is ImportMachine importMachine)
                                {
                                    importMachine.position = arcadeMachine1.position;
                                    Remove(arcadeMachine1);
                                    Add(importMachine);
                                    things.RefreshState();
                                    _challenges.Add(importMachine);
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
                int num = bareInitialize ? 1 : 0;
                foreach (ArcadeMachine arcadeMachine2 in things[typeof(ArcadeMachine)])
                    _challenges.Add(arcadeMachine2);
            }
            Profiles.active[0].ticketCount = Challenges.GetTicketCount(Profiles.active[0]);
            foreach (ArcadeFrame arcadeFrame in things[typeof(ArcadeFrame)])
                _frames.Add(arcadeFrame);
            foreach (ChallengeSaveData challengeSaveData in Challenges.GetAllSaveData())
            {
                if (challengeSaveData.frameID != "")
                {
                    ArcadeFrame frame = GetFrame(challengeSaveData.frameID);
                    if (frame != null)
                        frame.saveData = challengeSaveData;
                }
            }
            foreach (ArcadeMachine challenge in _challenges)
                challenge.unlocked = challenge.CheckUnlocked(false);
            _hud = new ArcadeHUD
            {
                alpha = 0f
            };
            Add(_hud);
            _unlockScreen = new UnlockScreen
            {
                alpha = 0f
            };
            Add(_unlockScreen);
            _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _pauseMenu = new UIMenu("@LWING@ARCADE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE  @SELECT@SELECT");
            _confirmMenu = new UIMenu("EXIT ARCADE?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            _advancedMenu = new UIMenu("@LWING@ADVANCED@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 260f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            _pausebox = new UIDivider(true, 0.8f);
            _pausebox.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_pauseGroup), UIAlign.Left), true);
            _pausebox.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
            _pausebox.leftSection.Add(new UIText("", Color.White), true);
            _pausebox.leftSection.Add(new UIMenuItem("|DGRED|EXIT ARCADE", new UIMenuActionOpenMenu(_pauseMenu, _confirmMenu), UIAlign.Left), true);
            _pausebox.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
            _pauseMenu.Add(_pausebox, true);
            _pauseMenu.Close();
            _pauseGroup.Add(_pauseMenu, false);
            Options.AddMenus(_pauseGroup);
            Options.openOnClose = _pauseMenu;
            _confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_confirmMenu, _pauseMenu), UIAlign.Left, backButton: true), true);
            _confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _quit)), true);
            _confirmMenu.Close();
            _pauseGroup.Add(_confirmMenu, false);
            _advancedMenu.Add(new UIText("|DGBLUE|SPEEDRUN SETTINGS", Color.White), true);
            _advancedMenu.Add(new UIText("", Color.White), true);
            _advancedMenu.Add(new UIText("If enabled, Speedrun Mode", Colors.DGBlue), true);
            _advancedMenu.Add(new UIText("will fix the random generator", Colors.DGBlue), true);
            _advancedMenu.Add(new UIText("to make target spawns", Colors.DGBlue), true);
            _advancedMenu.Add(new UIText("deterministic.", Colors.DGBlue), true);
            _advancedMenu.Add(new UIMenuItemToggle("SPEEDRUN MODE", field: new FieldBinding(DuckNetwork.core, "speedrunMode")), true);
            _advancedMenu.Add(new UIMenuItemToggle("MAX TROPHY", field: new FieldBinding(DuckNetwork.core, "speedrunMaxTrophy", max: 5f), multi: new List<string>()
            {
                "OFF",
                "@BRONZE@",
                "@SILVER@",
                "@GOLD@",
                "@PLATINUM@",
                "@DEVELOPER@"
            }, compressedMulti: true), true);
            _advancedMenu.Add(new UIText("", Color.White), true);
            _advancedMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_advancedMenu, _pauseMenu), backButton: true), true);
            _advancedMenu.Close();
            _pauseGroup.Add(_advancedMenu, false);
            _pauseGroup.isPauseMenu = true;
            _pauseGroup.Close();
            Add(_pauseGroup);
            _prizeTable = things[typeof(PrizeTable)].FirstOrDefault() as PrizeTable;
            _plugMachine = things[typeof(PlugMachine)].FirstOrDefault() as PlugMachine;
            if (_prizeTable == null)
                _prizeTable = new PrizeTable(730f, 124f);
            Chancy.activeChallenge = null;
            Chancy.atCounter = true;
            Chancy.lookingAtChallenge = false;
            basementWasUnlocked = Unlocks.IsUnlocked("BASEMENTKEY", Profiles.active[0]);
            Add(_prizeTable);
            Music.Play("Arcade");
            _exitDoor = new MetroidDoor(-192f, 320f);
            Add(_exitDoor);
            _followCam.hardLimitLeft = -192f;
        }

        public override void Terminate()
        {
        }
        public Vec2 SpawnPosition = Vec2.Zero;

        public override void Update()
        {
            ++MonoMain.timeInArcade;
            if (!_prevGotDev && Options.Data.gotDevMedal)
            {
                _prevGotDev = true;
                _pausebox.leftSection.Insert(new UIMenuItem("ADVANCED", new UIMenuActionOpenMenu(_pauseMenu, _advancedMenu), UIAlign.Left), 2, true);
            }
            if (_entering)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                if (Graphics.fade > 0.99f)
                {
                    _entering = false;
                    Graphics.fade = 1f;
                }
            }
            Options.openOnClose = _pauseMenu;
            if (_duck != null && _duck.profile != null && !_duck.profile.inputProfile.HasAnyConnectedDevice())
            {
                foreach (InputProfile defaultProfile in InputProfile.defaultProfiles)
                {
                    if (defaultProfile.HasAnyConnectedDevice())
                    {
                        InputProfile.SwapDefaultInputStrings(defaultProfile.name, _duck.profile.inputProfile.name);
                        InputProfile.ReassignDefaultInputProfiles();
                        _duck.profile.inputProfile = defaultProfile;
                        break;
                    }
                }
            }
            if (_duck != null && _duck.dead)
            {
                followCam.Clear();
                Remove(_duck);
                followCam.Remove(_duck);
                _duck = new Duck(SpawnPosition.x, SpawnPosition.y, _duck.profile);
                followCam.Add(_duck);
                Add(_duck);
                HUD.AddInputChangeDisplay(" Cmon Now That Was Dumb, Dont You Agree? ");
            }
            if (spawnKey)
            {
                if (spawnKeyWait > 0f) spawnKeyWait -= Maths.IncFrameTimer();
                else
                {
                    SFX.Play("ching");
                    spawnKey = false;
                    Key key = new Key(_prizeTable.x, _prizeTable.y)
                    {
                        vSpeed = -4f,
                        depth = _duck.depth + 50
                    };
                    if (DGRSettings.S_ParticleMultiplier != 0)
                    {
                        Add(SmallSmoke.New(key.x + Rando.Float(-4f, 4f), key.y + Rando.Float(-4f, 4f)));
                        Add(SmallSmoke.New(key.x + Rando.Float(-4f, 4f), key.y + Rando.Float(-4f, 4f)));
                        Add(SmallSmoke.New(key.x + Rando.Float(-4f, 4f), key.y + Rando.Float(-4f, 4f)));
                        Add(SmallSmoke.New(key.x + Rando.Float(-4f, 4f), key.y + Rando.Float(-4f, 4f)));
                    }
                    Add(key);
                }
            }
            Chancy.Update();
            if (_pendingSpawns != null && _pendingSpawns.Count > 0)
            {
                Duck pendingSpawn = _pendingSpawns[0];
                AddThing(pendingSpawn);
                _pendingSpawns.RemoveAt(0);
                _duck = pendingSpawn;
                _arcade = things[typeof(ArcadeMode)].First() as ArcadeMode;
                if (!_enteringCameraUpdated)
                {
                    _enteringCameraUpdated = true;
                    for (int index = 0; index < 200; ++index)
                        _followCam.Update();
                }
            }
            double num = Math.Min(1f, Math.Max(0f, (float)((1f - Layer.Game.fade) * 1.5f)));
            backgroundColor = Color.Black;
            if (UnlockScreen.open || ArcadeHUD.open)
            {
                foreach (Thing challenge in _challenges)
                    challenge.visible = false;
                _prizeTable.visible = false;
            }
            else
            {
                foreach (Thing challenge in _challenges)
                    challenge.visible = true;
                _prizeTable.visible = true;
            }
            if (_duck != null)
                _exitDoor._arcadeProfile = _duck.profile;
            if (_state == _desiredState && _state != ArcadeState.UnlockMachine && _state != ArcadeState.LaunchChallenge)
            {
                if (!_quitting)
                {
                    ArcadeHatConsole arcadeHatConsole = First<ArcadeHatConsole>();
                    if (Input.Pressed(Triggers.Start) && (arcadeHatConsole == null || !arcadeHatConsole.IsOpen()))
                    {
                        _pauseGroup.Open();
                        _pauseMenu.Open();
                        MonoMain.pauseMenu = _pauseGroup;
                        if (!_paused)
                        {
                            Music.Pause();
                            SFX.Play("pause", 0.6f);
                            _paused = true;
                            _duck.immobilized = true;
                        }
                        simulatePhysics = false;
                        return;
                    }
                    if (_paused && MonoMain.pauseMenu == null)
                    {
                        _paused = false;
                        SFX.Play("resume", 0.6f);
                        if (_quit.value)
                        {
                            _quitting = true;
                        }
                        else
                        {
                            Music.Resume();
                            _duck.immobilized = false;
                            simulatePhysics = true;
                        }
                    }
                }
                else
                {
                    Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.05f);
                    if (Graphics.fade > 0.01f)
                        return;
                    Chancy.StopShowingChallengeList();
                    if (editor != null)
                    {
                        current = editor;
                        return;
                    }
                    current = new TitleScreen();
                    return;
                }
            }
            if (_paused)
                return;
            if (_hud.launchChallenge)
                _desiredState = ArcadeState.LaunchChallenge;
            if (_desiredState != _state)
            {
                _duck.active = false;
                bool flag = false;
                if (_desiredState == ArcadeState.ViewChallenge)
                {
                    _duck.alpha = Lerp.FloatSmooth(_duck.alpha, 0f, 0.1f);
                    _followCam.manualViewSize = Lerp.FloatSmooth(_followCam.manualViewSize, 2f, 0.16f);
                    if (_followCam.manualViewSize < 30f)
                    {
                        Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        _hud.alpha = Lerp.Float(_hud.alpha, 1f, 0.08f);
                        if (_followCam.manualViewSize < 3f && _hud.alpha == 1f && Layer.Game.fade == 0f)
                            flag = true;
                    }
                }
                else if (_desiredState == ArcadeState.Normal)
                {
                    if (!_flipState)
                    {
                        _followCam.Clear();
                        _followCam.Add(_duck);
                        HUD.CloseAllCorners();
                    }
                    _duck.alpha = Lerp.FloatSmooth(_duck.alpha, 1f, 0.1f, 1.1f);
                    if (_state == ArcadeState.ViewChallenge || _state == ArcadeState.UnlockScreen)
                        _followCam.manualViewSize = Lerp.FloatSmooth(_followCam.manualViewSize, _followCam.viewSize, 0.14f, 1.05f);
                    Layer.Game.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    Layer.Background.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    _hud.alpha = Lerp.Float(_hud.alpha, 0f, 0.08f);
                    _unlockScreen.alpha = Lerp.Float(_unlockScreen.alpha, 0f, 0.08f);
                    if ((_followCam.manualViewSize < 0f || _followCam.manualViewSize == _followCam.viewSize) && _hud.alpha == 0f && Layer.Game.fade == 1f)
                    {
                        flag = true;
                        _followCam.manualViewSize = -1f;
                        _duck.alpha = 1f;
                    }
                    if (Unlockables.HasPendingUnlocks())
                        MonoMain.pauseMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
                }
                else if (_desiredState == ArcadeState.ViewSpecialChallenge || _desiredState == ArcadeState.ViewChallengeList || _desiredState == ArcadeState.ViewProfileSelector)
                {
                    if (!_flipState)
                    {
                        _followCam.Clear();
                        _followCam.Add(_duck);
                        HUD.CloseAllCorners();
                    }
                    _duck.alpha = Lerp.FloatSmooth(_duck.alpha, 1f, 0.1f, 1.1f);
                    if (_state == ArcadeState.ViewChallenge || _state == ArcadeState.UnlockScreen)
                        _followCam.manualViewSize = Lerp.FloatSmooth(_followCam.manualViewSize, _followCam.viewSize, 0.14f, 1.05f);
                    Layer.Game.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    Layer.Background.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    _hud.alpha = Lerp.Float(_hud.alpha, 0f, 0.08f);
                    _unlockScreen.alpha = Lerp.Float(_unlockScreen.alpha, 0f, 0.08f);
                    if ((_followCam.manualViewSize < 0f || _followCam.manualViewSize == _followCam.viewSize) && _hud.alpha == 0f && Layer.Game.fade == 1f)
                    {
                        flag = true;
                        _followCam.manualViewSize = -1f;
                        _duck.alpha = 1f;
                    }
                }
                else if (_desiredState == ArcadeState.UnlockMachine)
                {
                    if (!_flipState)
                    {
                        _followCam.Clear();
                        _followCam.Add(_unlockMachines[0]);
                        HUD.CloseAllCorners();
                    }
                    if (_state == ArcadeState.ViewChallenge)
                        _followCam.manualViewSize = Lerp.FloatSmooth(_followCam.manualViewSize, _followCam.viewSize, 0.14f, 1.05f);
                    _duck.alpha = Lerp.FloatSmooth(_duck.alpha, 1f, 0.1f, 1.1f);
                    Layer.Game.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    Layer.Background.fade = Lerp.Float(Layer.Game.fade, 1f, 0.05f);
                    _hud.alpha = Lerp.Float(_hud.alpha, 0f, 0.08f);
                    _unlockScreen.alpha = Lerp.Float(_unlockScreen.alpha, 0f, 0.08f);
                    _unlockMachineWait = 1f;
                    if ((_followCam.manualViewSize < 0f || _followCam.manualViewSize == _followCam.viewSize) && _hud.alpha == 0f && Layer.Game.fade == 1f)
                    {
                        flag = true;
                        _followCam.manualViewSize = -1f;
                        _duck.alpha = 1f;
                    }
                }
                else if (_desiredState == ArcadeState.LaunchChallenge)
                {
                    if (!_flipState)
                        HUD.CloseAllCorners();
                    Music.volume = Lerp.Float(Music.volume, 0f, 0.01f);
                    _hud.alpha = Lerp.Float(_hud.alpha, 0f, 0.02f);
                    _unlockScreen.alpha = Lerp.Float(_unlockScreen.alpha, 0f, 0.08f);
                    if (_hud.alpha == 0f)
                        flag = true;
                }
                if (_desiredState == ArcadeState.UnlockScreen)
                {
                    _duck.alpha = Lerp.FloatSmooth(_duck.alpha, 0f, 0.1f);
                    _followCam.manualViewSize = Lerp.FloatSmooth(_followCam.manualViewSize, 2f, 0.16f);
                    if (_followCam.manualViewSize < 30f)
                    {
                        Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        _unlockScreen.alpha = Lerp.Float(_unlockScreen.alpha, 1f, 0.08f);
                        if (_followCam.manualViewSize < 3f && _unlockScreen.alpha == 1f && Layer.Game.fade == 0f)
                            flag = true;
                    }
                }
                if (_desiredState == ArcadeState.Plug)
                    flag = true;
                _flipState = true;
                if (_launchedChallenge)
                {
                    Layer.Background.fade = 0f;
                    Layer.Game.fade = 0f;
                }
                if (flag)
                {
                    _flipState = false;
                    HUD.CloseAllCorners();
                    _state = _desiredState;
                    if (_state == ArcadeState.ViewChallenge)
                    {
                        if (_afterChallenge)
                        {
                            Music.Play("Arcade");
                            _afterChallenge = false;
                        }
                        _hud.MakeActive();
                        Add(_hud);
                        _duck.active = false;
                    }
                    else
                    {
                        if (_state == ArcadeState.LaunchChallenge)
                        {
                            currentArcade = this;
                            foreach (Thing thing in things[typeof(ChallengeConfetti)])
                                Remove(thing);
                            Music.Stop();
                            current = new ChallengeLevel(_hud.selected.challenge.fileName);
                            if (!launchSpecialChallenge)
                            {
                                _desiredState = ArcadeState.ViewChallenge;
                                _hud.launchChallenge = false;
                                _launchedChallenge = false;
                                _afterChallenge = true;
                                return;
                            }
                            _desiredState = ArcadeState.ViewSpecialChallenge;
                            _hud.launchChallenge = false;
                            _launchedChallenge = false;
                            _afterChallenge = true;
                            launchSpecialChallenge = false;
                            return;
                        }
                        if (_state != ArcadeState.UnlockMachine)
                        {
                            if (_state == ArcadeState.Normal)
                            {
                                _unlockMachines.Clear();
                                foreach (ArcadeMachine challenge in _challenges)
                                {
                                    if (challenge.CheckUnlocked())
                                        _unlockMachines.Add(challenge);
                                }
                                if (_unlockMachines.Count > 0)
                                {
                                    _desiredState = ArcadeState.UnlockMachine;
                                    return;
                                }
                                if (!basementWasUnlocked && Unlocks.IsUnlocked("BASEMENTKEY", Profiles.active[0]))
                                {
                                    spawnKey = true;
                                    basementWasUnlocked = true;
                                }
                                _duck.active = true;
                            }
                            else if (_state == ArcadeState.ViewSpecialChallenge)
                            {
                                _duck.active = false;
                                if (_afterChallenge)
                                {
                                    Music.Play("Arcade");
                                    _afterChallenge = false;
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
                                _duck.active = false;
                            }
                            else if (_state == ArcadeState.ViewProfileSelector)
                            {
                                _duck.active = false;
                                ArcadeHatConsole arcadeHatConsole = First<ArcadeHatConsole>();
                                if (arcadeHatConsole != null)
                                {
                                    HUD.CloseAllCorners();
                                    arcadeHatConsole.Open();
                                }
                            }
                            else if (_state == ArcadeState.ViewChallengeList)
                            {
                                _duck.active = false;
                                HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@ACCEPT");
                                HUD.AddCornerControl(HUDCorner.BottomLeft, "@CANCEL@BACK");
                            }
                            else if (_state == ArcadeState.UnlockScreen)
                            {
                                basementWasUnlocked = Unlocks.IsUnlocked("BASEMENTKEY", Profiles.active[0]);
                                _unlockScreen.MakeActive();
                                _duck.active = false;
                            }
                            else if (_state == ArcadeState.Plug)
                            {
                                Plug.Add("Yo! I'm |DGBLUE|[QC] WUMP|WHITE|, Chancy's Sister.^Machine behind me plays |DGYELLOW|user challenges|WHITE|, as well^as obscenely hard challenges I made myself!");
                                Plug.Add("And the machine behind this one is today's^|DGYELLOW|Imported Machine.");
                                Plug.Open();
                                _duck.active = false;
                            }
                        }
                    }
                }
            }
            else if (_state == ArcadeState.Normal || _state == ArcadeState.UnlockMachine)
            {
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, 1f, 0.08f);
                Layer.Background.fade = Lerp.Float(Layer.Game.fade, 1f, 0.08f);
                _hud.alpha = Lerp.Float(_hud.alpha, 0f, 0.08f);
                if (_state == ArcadeState.Normal)
                {
                    object obj = null;
                    foreach (ArcadeMachine challenge in _challenges)
                    {
                        double length = (_duck.position - challenge.position).length;
                        if (challenge.hover)
                        {
                            obj = challenge;
                            if (Input.Pressed(Triggers.Shoot))
                            {
                                _hud.activeChallengeGroup = challenge.data;
                                _desiredState = ArcadeState.ViewChallenge;
                                _followCam.manualViewSize = _followCam.viewSize;
                                _followCam.Clear();
                                _followCam.Add(challenge);
                                HUD.CloseAllCorners();
                                _hoverMachine = null;
                                _hoverThing = null;
                                return;
                            }
                        }
                        if (_prizeTable.hover)
                        {
                            obj = _prizeTable;
                            if (Input.Pressed(Triggers.Shoot))
                            {
                                _desiredState = ArcadeState.UnlockScreen;
                                _followCam.manualViewSize = _followCam.viewSize;
                                _followCam.Clear();
                                _followCam.Add(_prizeTable);
                                HUD.CloseAllCorners();
                                _hoverMachine = null;
                                _hoverThing = null;
                                return;
                            }
                        }
                        else if (_plugMachine != null && _plugMachine.hover)
                        {
                            obj = _plugMachine;
                            if (Input.Pressed(Triggers.Shoot))
                            {
                                _desiredState = ArcadeState.Plug;
                                _followCam.manualViewSize = _followCam.viewSize;
                                _followCam.Clear();
                                _followCam.Add(_plugMachine);
                                HUD.CloseAllCorners();
                                _hoverMachine = null;
                                _hoverThing = null;
                                return;
                            }
                        }
                    }
                    if (Chancy.hover && Input.Pressed(Triggers.Shoot))
                    {
                        _desiredState = ArcadeState.ViewSpecialChallenge;
                        HUD.CloseAllCorners();
                        _hoverMachine = null;
                        _hoverThing = null;
                        Chancy.hover = false;
                        Chancy.lookingAtChallenge = true;
                        Chancy.OpenChallengeView();
                        return;
                    }
                    ArcadeHatConsole arcadeHatConsole = First<ArcadeHatConsole>();
                    if (arcadeHatConsole != null && Input.Pressed(Triggers.Shoot) && arcadeHatConsole.hover)
                    {
                        _desiredState = ArcadeState.ViewProfileSelector;
                        HUD.CloseAllCorners();
                        _hoverMachine = null;
                        _hoverThing = null;
                        return;
                    }
                    Chancy.hover = false;
                    if (!Chancy.atCounter)
                    {
                        if ((_duck.position - Chancy.standingPosition).length < 22f)
                        {
                            obj = Chancy.context;
                            Chancy.hover = true;
                        }
                        if (Chancy.standingPosition.x < Layer.Game.camera.left - 16f || Chancy.standingPosition.x > Layer.Game.camera.right + 16f || Chancy.standingPosition.y < Layer.Game.camera.top - 16f || Chancy.standingPosition.y > Layer.Game.camera.bottom + 16f)
                        {
                            Chancy.atCounter = true;
                            Chancy.activeChallenge = null;
                        }
                    }
                    else if (_prizeTable.hoverChancyChallenge)
                    {
                        obj = _arcade;
                        if (Input.Pressed(Triggers.Shoot))
                        {
                            _desiredState = ArcadeState.ViewChallengeList;
                            HUD.CloseAllCorners();
                            Chancy.OpenChallengeList();
                            _hoverMachine = null;
                            _hoverThing = null;
                            Chancy.hover = false;
                            Chancy.lookingAtList = true;
                            return;
                        }
                    }
                    if (_hoverThing != obj)
                    {
                        HUD.CloseAllCorners();
                        _hoverThing = obj;
                        _hoverMachine = !(_hoverThing is ArcadeMachine) ? null : obj as ArcadeMachine;
                        if (_hoverMachine != null)
                        {
                            HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@PLAY");
                            string text = _hoverMachine.data.GetNameForDisplay() + " ";
                            foreach (string challenge1 in _hoverMachine.data.challenges)
                            {
                                ChallengeData challenge2 = Challenges.GetChallenge(challenge1);
                                if (challenge2 != null)
                                {
                                    ChallengeSaveData saveData = _duck.profile.GetSaveData(challenge2.levelID);
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
                        else if (_prizeTable.hover)
                        {
                            if (_prizeTable.hoverChancyChallenge)
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
                                    if (_prizeTable.hoverChancyChallenge)
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
                else if (_state == ArcadeState.UnlockMachine)
                {
                    _unlockMachineWait -= 0.02f;
                    if (_unlockMachineWait < 0f)
                    {
                        if (_unlockingMachine)
                        {
                            _unlockingMachine = false;
                            _followCam.Clear();
                            _followCam.Add(_unlockMachines[0]);
                            _unlockMachineWait = 1f;
                        }
                        else if (_unlockMachines.Count > 0)
                        {
                            _unlockMachines[0].unlocked = true;
                            _unlockMachines.RemoveAt(0);
                            _unlockingMachine = _unlockMachines.Count > 0;
                            SFX.Play("lightTurnOn", pitch: Rando.Float(-0.1f, 0.1f));
                            _unlockMachineWait = 1f;
                        }
                        else
                            _desiredState = ArcadeState.Normal;
                    }
                }
            }
            else if (_state == ArcadeState.ViewChallenge)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0f, 0.05f);
                Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0f, 0.05f);
                _hud.alpha = Lerp.Float(_hud.alpha, 1f, 0.05f);
                if (_hud.quitOut)
                {
                    _hud.quitOut = false;
                    _desiredState = ArcadeState.Normal;
                    if (Chancy.activeChallenge == null)
                    {
                        List<ChallengeData> chancyChallenges = Challenges.GetEligibleIncompleteChancyChallenges(Profiles.active[0]);
                        if (chancyChallenges.Count > 0)
                        {
                            Vec2 position = _duck.position;
                            ArcadeMachine arcadeMachine = Nearest<ArcadeMachine>(_duck.x, _duck.y);
                            if (arcadeMachine != null)
                                position = arcadeMachine.position;
                            chancyChallenges.OrderBy(v => v.GetRequirementValue());
                            Chancy.AddProposition(chancyChallenges[chancyChallenges.Count - 1], position);
                        }
                    }
                }
            }
            else if (_state == ArcadeState.UnlockScreen)
            {
                if (_unlockScreen.quitOut)
                {
                    _unlockScreen.quitOut = false;
                    _desiredState = ArcadeState.Normal;
                }
            }
            else if (_state == ArcadeState.ViewSpecialChallenge)
            {
                if (!launchSpecialChallenge)
                {
                    Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                    if (Input.Pressed(Triggers.Cancel))
                    {
                        if (returnToChallengeList)
                        {
                            _desiredState = ArcadeState.ViewChallengeList;
                            Chancy.hover = false;
                            Chancy.lookingAtList = true;
                        }
                        else
                            _desiredState = ArcadeState.Normal;
                        Chancy.lookingAtChallenge = false;
                        HUD.CloseAllCorners();
                        SFX.Play("consoleCancel");
                        return;
                    }
                    if (Input.Pressed(Triggers.Select))
                    {
                        launchSpecialChallenge = true;
                        SFX.Play("consoleSelect");
                        return;
                    }
                }
                else
                {
                    Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.05f);
                    if (Graphics.fade < 0.01f)
                    {
                        _hud.launchChallenge = true;
                        _hud.selected = new ChallengeCard(0f, 0f, Chancy.activeChallenge);
                        HUD.CloseAllCorners();
                    }
                }
            }
            else if (_state == ArcadeState.ViewChallengeList)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                if (Input.Pressed(Triggers.Cancel))
                {
                    _desiredState = ArcadeState.Normal;
                    Chancy.lookingAtChallenge = false;
                    Chancy.lookingAtList = false;
                    HUD.CloseAllCorners();
                    SFX.Play("consoleCancel");
                    return;
                }
                if (Input.Pressed(Triggers.Select))
                {
                    Chancy.AddProposition(Chancy.selectedChallenge, Chancy.standingPosition);
                    returnToChallengeList = true;
                    _desiredState = ArcadeState.ViewSpecialChallenge;
                    HUD.CloseAllCorners();
                    _hoverMachine = null;
                    _hoverThing = null;
                    Chancy.hover = false;
                    Chancy.lookingAtChallenge = true;
                    Chancy.lookingAtList = false;
                    Chancy.OpenChallengeView();
                }
            }
            else if (_state == ArcadeState.ViewProfileSelector)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                ArcadeHatConsole arcadeHatConsole = First<ArcadeHatConsole>();
                if (arcadeHatConsole != null && !arcadeHatConsole.IsOpen())
                {
                    foreach (ArcadeMachine challenge in _challenges)
                        challenge.unlocked = challenge.CheckUnlocked(false);
                    _unlockMachines.Clear();
                    UpdateDefault();
                    _desiredState = ArcadeState.Normal;
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
                if (_speedClock == null)
                    _speedClock = new Sprite("speedrunClock");
                if (DuckNetwork.core.speedrunMode)
                    Graphics.Draw(_speedClock, 4f, 4f);
            }
            if (layer == Layer.Game)
                Chancy.DrawGameLayer();
            base.PostDrawLayer(layer);
        }
    }
}
