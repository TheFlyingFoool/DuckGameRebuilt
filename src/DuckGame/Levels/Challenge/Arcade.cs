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

        public FollowCam followCam => this._followCam;

        public Arcade()
        {
            this._followCam = new FollowCam
            {
                lerpMult = 2f
            };
            this.camera = _followCam;
            DeathmatchLevel.started = true;
        }

        public override void Initialize()
        {
            this._background = new SpriteThing(313f, -40f, new Sprite("arcade/arcadeOuya"))
            {
                center = new Vec2(0.0f, 0.0f),
                layer = Layer.Background
            };
            this._duck = new Duck(730f, 100f, Profiles.active[0]);
            Level.Add(_background);
            Level.Add(_duck);
            this._followCam.Add(_duck);
            Chancy.Add("SUP MOTHARFUCKAR :P");
            Level.Add(new Block(0.0f, 187f, 295f, 53f));
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
            Level.Add(new Block(787f, 0.0f, 64f, 300f));
            Level.Add(new Block(-16f, 0.0f, 21f, 300f));
            Level.Add(new Platform(648f, 131f, 12f, 8f));
            Level.Add(new Platform(640f, 123f, 12f, 8f));
            Level.Add(new Platform(632f, 115f, 12f, 8f));
            Level.Add(new Block(624f, 107f, 12f, 8f));
            Level.Add(new Block(616f, 99f, 12f, 8f));
            Level.Add(new Block(-100f, 91f, 720f, 14f));
            Level.Add(new Block(251f, 83f, 268f, 10f));
            Level.Add(new Block(259f, 75f, 252f, 10f));
            Level.Add(new Block(254f, 0.0f, 64f, 300f));
            List<Vec2> vec2List = new List<Vec2>();
            vec2List.Add(new Vec2(380f, 186f));
            vec2List.Add(new Vec2(520f, 170f));
            vec2List.Add(new Vec2(565f, 74f));
            vec2List.Add(new Vec2(375f, 58f));
            vec2List.Add(new Vec2(455f, 58f));
            Vec2 vec2_1 = vec2List[this._challenges.Count];
            Vec2 vec2_2 = vec2List[this._challenges.Count];
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
            this._challenges.Add(arcadeMachine1);
            Vec2 vec2_3 = vec2List[this._challenges.Count];
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
            this._challenges.Add(arcadeMachine2);
            Vec2 vec2_4 = vec2List[this._challenges.Count];
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
            this._challenges.Add(arcadeMachine3);
            Vec2 vec2_5 = vec2List[this._challenges.Count];
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
            this._challenges.Add(arcadeMachine4);
            Vec2 vec2_6 = vec2List[this._challenges.Count];
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
            this._challenges.Add(arcadeMachine5);
            this._prizeTable = new PrizeTable(730f, 124f);
            Level.Add(_prizeTable);
            this._hud = new ArcadeHUD
            {
                alpha = 0.0f
            };
            this._unlockScreen = new UnlockScreen
            {
                alpha = 0.0f
            };
            Level.Add(_unlockScreen);
            this._pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0.0f, 0.0f);
            this._pauseMenu = new UIMenu("@LWING@ARCADE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE  @SELECT@SELECT");
            this._confirmMenu = new UIMenu("EXIT ARCADE?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            UIDivider component = new UIDivider(true, 0.8f);
            component.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(this._pauseGroup), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
            component.leftSection.Add(new UIMenuItem("EXIT ARCADE", new UIMenuActionOpenMenu(_pauseMenu, _confirmMenu), UIAlign.Left), true);
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
            this._pauseGroup.Close();
            this._pauseGroup.isPauseMenu = true;
            Level.Add(_pauseGroup);
            Music.Play(nameof(Arcade));
            base.Initialize();
        }

        public override void Terminate()
        {
        }

        public override void Update()
        {
            this.backgroundColor = Color.Black;
            if (UnlockScreen.open || ArcadeHUD.open)
            {
                this._background.visible = false;
                foreach (Thing challenge in this._challenges)
                    challenge.visible = false;
                this._prizeTable.visible = false;
            }
            else
            {
                this._background.visible = true;
                foreach (Thing challenge in this._challenges)
                    challenge.visible = true;
                this._prizeTable.visible = true;
            }
            if (this._state == this._desiredState && this._state != ArcadeState.UnlockMachine && this._state != ArcadeState.LaunchChallenge)
            {
                if (!this._quitting)
                {
                    if (Input.Pressed("START"))
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
                    Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.02f);
                    if (Graphics.fade <= 0.01f)
                        Level.current = new TitleScreen();
                }
            }
            if (this._paused)
                return;
            this._hud.Update();
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
                    if (_followCam.manualViewSize < 30f)
                    {
                        Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0f, 0.08f);
                        this._hud.alpha = Lerp.Float(this._hud.alpha, 1f, 0.08f);
                        if (_followCam.manualViewSize < 3f && this._hud.alpha == 1f && Layer.Game.fade == 0f)
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
                    this._hud.alpha = Lerp.Float(this._hud.alpha, 0.0f, 0.08f);
                    this._unlockScreen.alpha = Lerp.Float(this._unlockScreen.alpha, 0.0f, 0.08f);
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
                    this._hud.alpha = Lerp.Float(this._hud.alpha, 0.0f, 0.08f);
                    this._unlockScreen.alpha = Lerp.Float(this._unlockScreen.alpha, 0.0f, 0.08f);
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
                    Music.volume = Lerp.Float(Music.volume, 0.0f, 0.01f);
                    this._hud.alpha = Lerp.Float(this._hud.alpha, 0.0f, 0.02f);
                    this._unlockScreen.alpha = Lerp.Float(this._unlockScreen.alpha, 0.0f, 0.08f);
                    if ((double)this._hud.alpha == 0.0)
                        flag = true;
                }
                if (this._desiredState == ArcadeState.UnlockScreen)
                {
                    this._duck.alpha = Lerp.FloatSmooth(this._duck.alpha, 0.0f, 0.1f);
                    this._followCam.manualViewSize = Lerp.FloatSmooth(this._followCam.manualViewSize, 2f, 0.16f);
                    if (_followCam.manualViewSize < 30.0)
                    {
                        Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0.0f, 0.08f);
                        Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0.0f, 0.08f);
                        this._unlockScreen.alpha = Lerp.Float(this._unlockScreen.alpha, 1f, 0.08f);
                        if (_followCam.manualViewSize < 3.0 && (double)this._unlockScreen.alpha == 1.0 && (double)Layer.Game.fade == 0.0)
                            flag = true;
                    }
                }
                this._flipState = true;
                if (this._launchedChallenge)
                {
                    Layer.Background.fade = 0.0f;
                    Layer.Game.fade = 0.0f;
                }
                if (!flag)
                    return;
                this._flipState = false;
                HUD.CloseAllCorners();
                this._state = this._desiredState;
                if (this._state == ArcadeState.ViewChallenge)
                {
                    if (this._afterChallenge)
                    {
                        Music.Play(nameof(Arcade));
                        this._afterChallenge = false;
                        DuckFile.FlagForBackup();
                    }
                    this._hud.MakeActive();
                    this._duck.active = false;
                }
                else if (this._state == ArcadeState.LaunchChallenge)
                {
                    Arcade.currentArcade = this;
                    foreach (Thing thing in this.things[typeof(ChallengeConfetti)])
                        Level.Remove(thing);
                    Music.Stop();
                    Level.current = new ChallengeLevel(this._hud.selected.challenge.fileName);
                    this._desiredState = ArcadeState.ViewChallenge;
                    this._hud.launchChallenge = false;
                    this._launchedChallenge = false;
                    this._afterChallenge = true;
                }
                else
                {
                    if (this._state == ArcadeState.UnlockMachine)
                        return;
                    if (this._state == ArcadeState.Normal)
                    {
                        this._unlockMachines.Clear();
                        foreach (ArcadeMachine challenge in this._challenges)
                        {
                            if (challenge.CheckUnlocked())
                                this._unlockMachines.Add(challenge);
                        }
                        if (this._unlockMachines.Count > 0)
                            this._desiredState = ArcadeState.UnlockMachine;
                        else
                            this._duck.active = true;
                    }
                    else if (this._state == ArcadeState.UnlockScreen)
                    {
                        this._unlockScreen.MakeActive();
                        this._duck.active = false;
                    }
                    else
                    {
                        if (this._state != ArcadeState.ViewSpecialChallenge)
                            return;
                        if (this._afterChallenge)
                        {
                            Music.Play(nameof(Arcade));
                            this._afterChallenge = false;
                            DuckFile.FlagForBackup();
                        }
                        Chancy.afterChallenge = true;
                        Chancy.afterChallengeWait = 1f;
                        this._duck.active = false;
                    }
                }
            }
            else if (this._state == ArcadeState.Normal || this._state == ArcadeState.UnlockMachine)
            {
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, 1f, 0.08f);
                Layer.Background.fade = Lerp.Float(Layer.Game.fade, 1f, 0.08f);
                this._hud.alpha = Lerp.Float(this._hud.alpha, 0.0f, 0.08f);
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
                    }
                    if (this._hoverThing == obj)
                        return;
                    HUD.CloseAllCorners();
                    this._hoverThing = obj;
                    this._hoverMachine = !(this._hoverThing is ArcadeMachine) ? null : obj as ArcadeMachine;
                    if (this._hoverMachine != null)
                    {
                        HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@PLAY", this._duck.inputProfile);
                        string text = this._hoverMachine.data.GetNameForDisplay() + " ";
                        foreach (string challenge in this._hoverMachine.data.challenges)
                        {
                            ChallengeSaveData saveData = this._duck.profile.GetSaveData(Challenges.GetChallenge(challenge).levelID);
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
                        if (!this._prizeTable.hover)
                            return;
                        HUD.AddCornerControl(HUDCorner.BottomRight, "@SHOOT@CHANCY", this._duck.inputProfile);
                        HUD.AddCornerCounter(HUDCorner.BottomMiddle, "@TICKET@ ", new FieldBinding(Profiles.active[0], "ticketCount"), animateCount: true);
                    }
                }
                else
                {
                    if (this._state != ArcadeState.UnlockMachine)
                        return;
                    this._unlockMachineWait -= 0.02f;
                    if (_unlockMachineWait >= 0.0)
                        return;
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
            else if (this._state == ArcadeState.ViewChallenge)
            {
                Layer.Game.fade = Lerp.Float(Layer.Game.fade, 0.0f, 0.05f);
                Layer.Background.fade = Lerp.Float(Layer.Game.fade, 0.0f, 0.05f);
                this._hud.alpha = Lerp.Float(this._hud.alpha, 1f, 0.05f);
                if (!this._hud.quitOut)
                    return;
                this._hud.quitOut = false;
                this._desiredState = ArcadeState.Normal;
            }
            else
            {
                if (this._state != ArcadeState.UnlockScreen || !this._unlockScreen.quitOut)
                    return;
                this._unlockScreen.quitOut = false;
                this._desiredState = ArcadeState.Normal;
            }
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD)
                this._hud.Draw();
            base.PostDrawLayer(layer);
        }
    }
}
