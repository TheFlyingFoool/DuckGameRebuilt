// Decompiled with JetBrains decompiler
// Type: DuckGame.Arcade
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class Arcade : Level
    {
        protected FollowCam _followCam;
        private ArcadeState _state;
        private ArcadeState _desiredState;
        private ArcadeHUD _hud;
        private UnlockScreen _unlockScreen;
        private List<ArcadeMachine> _unlockMachines = new List<ArcadeMachine>();
        private SpriteThing _background;
        private bool _unlockingMachine;
        private List<ArcadeMachine> _challenges = new List<ArcadeMachine>();
        private PrizeTable _prizeTable;
        private Duck _duck;
        private UIComponent _pauseGroup;
        private UIMenu _pauseMenu;
        private UIMenu _confirmMenu;
        private object _hoverThing;
        private ArcadeMachine _hoverMachine;
        public static Arcade currentArcade;
        private bool _launchedChallenge;
        private bool _flipState;
        private float _unlockMachineWait = 1f;
        private bool _paused;
        private bool _quitting;
        private MenuBoolean _quit = new MenuBoolean();
        private bool _afterChallenge;

        public FollowCam followCam => _followCam;

        public Arcade()
        {
            _followCam = new FollowCam
            {
                lerpMult = 2f
            };
            camera = _followCam;
            DeathmatchLevel.started = true;
        }

        public override void Initialize()
        {
            _background = new SpriteThing(313f, -40f, new Sprite("arcade/arcadeOuya"))
            {
                center = new Vec2(0f, 0f),
                layer = Layer.Background
            };
            _duck = new Duck(730f, 100f, Profiles.active[0]);
            Level.Add(_background);
            Level.Add(_duck);
            _followCam.Add(_duck);
            Chancy.Add("SUP MOTHARFUCKAR :P");
            Level.Add(new Block(0f, 187f, 295f, 53f));
            Level.Add(new Block(289f, 195f, 14f, 45f));
            Level.Add(new Block(290f, 203f, 190f, 37f));
            Level.Add(new Block(467f, 195f, 17f, 45f));
            Level.Add(new Block(475f, 187f, 217f, 53f));
            Level.Add(new Block(639f, 179f, 32f, 16f));
            Level.Add(new Block(647f, 171f, 32f, 16f));
            Level.Add(new Block(655f, 163f, 32f, 16f));
            Level.Add(new Block(663f, 155f, 32f, 16f));
            Level.Add(new Block(671f, 147f, 32f, 16f));
            Level.Add(new Block(679f, 139f, 124f, 16f));
            Level.Add(new Block(787f, 0f, 64f, 300f));
            Level.Add(new Block(-16f, 0f, 21f, 300f));
            Level.Add(new Platform(648f, 131f, 12f, 8f));
            Level.Add(new Platform(640f, 123f, 12f, 8f));
            Level.Add(new Platform(632f, 115f, 12f, 8f));
            Level.Add(new Block(624f, 107f, 12f, 8f));
            Level.Add(new Block(616f, 99f, 12f, 8f));
            Level.Add(new Block(-100f, 91f, 720f, 14f));
            Level.Add(new Block(251f, 83f, 268f, 10f));
            Level.Add(new Block(259f, 75f, 252f, 10f));
            Level.Add(new Block(254f, 0f, 64f, 300f));
            List<Vec2> vec2List = new List<Vec2>();
            vec2List.Add(new Vec2(380f, 186f));
            vec2List.Add(new Vec2(520f, 170f));
            vec2List.Add(new Vec2(565f, 74f));
            vec2List.Add(new Vec2(375f, 58f));
            vec2List.Add(new Vec2(455f, 58f));
            Vec2 vec2_1 = vec2List[_challenges.Count];
            Vec2 vec2_2 = vec2List[_challenges.Count];
            ArcadeMachine arcadeMachine1 = new ArcadeMachine(vec2_2.x, vec2_2.y, new ChallengeGroup()
            {
                name = "TARGETS",
                challenges = {
          "challenge/targets01",
          "challenge/targets03ouya",
          "challenge/targets02ouya"
        },
                trophiesRequired = 0
            }, 0)
            {
                lightColor = 2,
                unlocked = true
            };
            Level.Add(arcadeMachine1);
            _challenges.Add(arcadeMachine1);
            Vec2 vec2_3 = vec2List[_challenges.Count];
            ArcadeMachine arcadeMachine2 = new ArcadeMachine(vec2_3.x, vec2_3.y, new ChallengeGroup()
            {
                name = "VARIETY ZONE",
                challenges = {
          "challenge/obstacle",
          "challenge/shootout02",
          "challenge/jetpack02"
        },
                trophiesRequired = 0
            }, 6)
            {
                lightColor = 1
            };
            Level.Add(arcadeMachine2);
            _challenges.Add(arcadeMachine2);
            Vec2 vec2_4 = vec2List[_challenges.Count];
            ArcadeMachine arcadeMachine3 = new ArcadeMachine(vec2_4.x, vec2_4.y, new ChallengeGroup()
            {
                name = "TELEPORTER",
                challenges = {
          "challenge/tele02",
          "challenge/tele01",
          "challenge/tele03"
        },
                trophiesRequired = 1
            }, 4)
            {
                lightColor = 1
            };
            Level.Add(arcadeMachine3);
            _challenges.Add(arcadeMachine3);
            Vec2 vec2_5 = vec2List[_challenges.Count];
            ArcadeMachine arcadeMachine4 = new ArcadeMachine(vec2_5.x, vec2_5.y, new ChallengeGroup()
            {
                name = "WEAPON TRAINING",
                challenges = {
          "challenge/magnumouya",
          "challenge/chaingunouya",
          "challenge/sniper"
        },
                trophiesRequired = 4
            }, 5)
            {
                lightColor = 2
            };
            Level.Add(arcadeMachine4);
            _challenges.Add(arcadeMachine4);
            Vec2 vec2_6 = vec2List[_challenges.Count];
            ArcadeMachine arcadeMachine5 = new ArcadeMachine(vec2_6.x, vec2_6.y, new ChallengeGroup()
            {
                name = "VARIETY ZONE 2",
                challenges = {
          "challenge/ball01",
          "challenge/glass01ouya",
          "challenge/grapple04"
        },
                trophiesRequired = 9
            }, 8)
            {
                lightColor = 1
            };
            Level.Add(arcadeMachine5);
            _challenges.Add(arcadeMachine5);
            _prizeTable = new PrizeTable(730f, 124f);
            Level.Add(_prizeTable);
            _hud = new ArcadeHUD
            {
                alpha = 0f
            };
            _unlockScreen = new UnlockScreen
            {
                alpha = 0f
            };
            Level.Add(_unlockScreen);
            _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _pauseMenu = new UIMenu("@LWING@ARCADE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE  @SELECT@SELECT");
            _confirmMenu = new UIMenu("EXIT ARCADE?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            UIDivider component = new UIDivider(true, 0.8f);
            component.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_pauseGroup), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("EXIT ARCADE", new UIMenuActionOpenMenu(_pauseMenu, _confirmMenu), UIAlign.Left), true);
            component.rightSection.Add(new UIImage("pauseIcons", UIAlign.Right), true);
            _pauseMenu.Add(component, true);
            _pauseMenu.Close();
            _pauseGroup.Add(_pauseMenu, false);
            Options.AddMenus(_pauseGroup);
            Options.openOnClose = _pauseMenu;
            _confirmMenu.Add(new UIMenuItem("NO!", new UIMenuActionOpenMenu(_confirmMenu, _pauseMenu), UIAlign.Left, backButton: true), true);
            _confirmMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuSetBoolean(_pauseGroup, _quit)), true);
            _confirmMenu.Close();
            _pauseGroup.Add(_confirmMenu, false);
            _pauseGroup.Close();
            _pauseGroup.isPauseMenu = true;
            Level.Add(_pauseGroup);
            Music.Play(nameof(Arcade));
            base.Initialize();
        }

        public override void Terminate()
        {
        }

        public override void Update()
        {
            backgroundColor = Color.Black;
            if (UnlockScreen.open || ArcadeHUD.open)
            {
                _background.visible = false;
                foreach (Thing challenge in _challenges)
                    challenge.visible = false;
                _prizeTable.visible = false;
            }
            else
            {
                _background.visible = true;
                foreach (Thing challenge in _challenges)
                    challenge.visible = true;
                _prizeTable.visible = true;
            }
            if (_state == _desiredState && _state != ArcadeState.UnlockMachine && _state != ArcadeState.LaunchChallenge)
            {
                if (!_quitting)
                {
                    if (Input.Pressed("START"))
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
                    Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.02f);
                    if (Graphics.fade <= 0.01f)
                        Level.current = new TitleScreen();
                }
            }
            if (_paused)
                return;
            _hud.Update();
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
                    if ((_followCam.manualViewSize < 0.0 || _followCam.manualViewSize == _followCam.viewSize) && _hud.alpha == 0f && Layer.Game.fade == 1f)
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
                    if ((_followCam.manualViewSize < 0.0 || _followCam.manualViewSize == _followCam.viewSize) && _hud.alpha == 0f && Layer.Game.fade == 1f)
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
                    if (_followCam.manualViewSize < 30.0)
                    {
                        Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        _unlockScreen.alpha = Lerp.Float(_unlockScreen.alpha, 1f, 0.08f);
                        if (_followCam.manualViewSize < 3f && _unlockScreen.alpha == 1f && Layer.Game.fade == 0f)
                            flag = true;
                    }
                }
                _flipState = true;
                if (_launchedChallenge)
                {
                    Layer.Background.fade = 0f;
                    Layer.Game.fade = 0f;
                }
                if (!flag)
                    return;
                _flipState = false;
                HUD.CloseAllCorners();
                _state = _desiredState;
                if (_state == ArcadeState.ViewChallenge)
                {
                    if (_afterChallenge)
                    {
                        Music.Play(nameof(Arcade));
                        _afterChallenge = false;
                        DuckFile.FlagForBackup();
                    }
                    _hud.MakeActive();
                    _duck.active = false;
                }
                else if (_state == ArcadeState.LaunchChallenge)
                {
                    Arcade.currentArcade = this;
                    foreach (Thing thing in things[typeof(ChallengeConfetti)])
                        Level.Remove(thing);
                    Music.Stop();
                    Level.current = new ChallengeLevel(_hud.selected.challenge.fileName);
                    _desiredState = ArcadeState.ViewChallenge;
                    _hud.launchChallenge = false;
                    _launchedChallenge = false;
                    _afterChallenge = true;
                }
                else
                {
                    if (_state == ArcadeState.UnlockMachine)
                        return;
                    if (_state == ArcadeState.Normal)
                    {
                        _unlockMachines.Clear();
                        foreach (ArcadeMachine challenge in _challenges)
                        {
                            if (challenge.CheckUnlocked())
                                _unlockMachines.Add(challenge);
                        }
                        if (_unlockMachines.Count > 0)
                            _desiredState = ArcadeState.UnlockMachine;
                        else
                            _duck.active = true;
                    }
                    else if (_state == ArcadeState.UnlockScreen)
                    {
                        _unlockScreen.MakeActive();
                        _duck.active = false;
                    }
                    else
                    {
                        if (_state != ArcadeState.ViewSpecialChallenge)
                            return;
                        if (_afterChallenge)
                        {
                            Music.Play(nameof(Arcade));
                            _afterChallenge = false;
                            DuckFile.FlagForBackup();
                        }
                        Chancy.afterChallenge = true;
                        Chancy.afterChallengeWait = 1f;
                        _duck.active = false;
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
                            if (Input.Pressed("SHOOT"))
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
                            if (Input.Pressed("SHOOT"))
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
                    }
                    if (_hoverThing == obj)
                        return;
                    HUD.CloseAllCorners();
                    _hoverThing = obj;
                    _hoverMachine = !(_hoverThing is ArcadeMachine) ? null : obj as ArcadeMachine;
                    if (_hoverMachine != null)
                    {
                        HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@PLAY", _duck.inputProfile);
                        string text = _hoverMachine.data.GetNameForDisplay() + " ";
                        foreach (string challenge in _hoverMachine.data.challenges)
                        {
                            ChallengeSaveData saveData = _duck.profile.GetSaveData(Challenges.GetChallenge(challenge).levelID);
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
                        HUD.AddCornerMessage(HUDCorner.TopRight, text);
                    }
                    else
                    {
                        if (!_prizeTable.hover)
                            return;
                        HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@CHANCY", _duck.inputProfile);
                        HUD.AddCornerCounter(HUDCorner.BottomMiddle, "@TICKET@ ", new FieldBinding(Profiles.active[0], "ticketCount"), animateCount: true);
                    }
                }
                else
                {
                    if (_state != ArcadeState.UnlockMachine)
                        return;
                    _unlockMachineWait -= 0.02f;
                    if (_unlockMachineWait >= 0.0)
                        return;
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
            else if (_state == ArcadeState.ViewChallenge)
            {
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0f, 0.05f);
                Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0f, 0.05f);
                _hud.alpha = Lerp.Float(_hud.alpha, 1f, 0.05f);
                if (!_hud.quitOut)
                    return;
                _hud.quitOut = false;
                _desiredState = ArcadeState.Normal;
            }
            else
            {
                if (_state != ArcadeState.UnlockScreen || !_unlockScreen.quitOut)
                    return;
                _unlockScreen.quitOut = false;
                _desiredState = ArcadeState.Normal;
            }
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD)
                _hud.Draw();
            base.PostDrawLayer(layer);
        }
    }
}
