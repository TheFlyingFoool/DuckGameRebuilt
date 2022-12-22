using System.Collections.Generic;

namespace DuckGame
{
    public partial class DevTestLev : XMLLevel
    {
        private Duck _duck;
        protected FollowCam _followCam;

        private bool _paused;
        private bool _quitting;
        private UIComponent _pauseGroup;

        private UIMenu _pauseMenu;
        private UIMenu _confirmMenu;
        private UIDivider _pausebox;
        private MenuBoolean _quit = new MenuBoolean();
        public List<ArcadeMachine> _challenges = new List<ArcadeMachine>();
        public FollowCam followCam
        {
            get
            {
                return this._followCam;
            }
        }
        public DevTestLev() : base(Content.ReloadAndGetLevelID("devtestlev"))
        {
            _followCam = new FollowCam
            {
                lerpMult = 2f,
                startCentered = false
            };
            camera = _followCam;
        }
        private List<Duck> _pendingSpawns;
        private List<Duck> spawnedducks = new List<Duck>();
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
            }
            followCam.Adjust();

   
            //_hud = new ArcadeHUD
            //{
            //    alpha = 0f
            //};
            //Add(_hud);
            _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _pauseMenu = new UIMenu("@LWING@DEVLEVEL@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE  @SELECT@SELECT");
            _confirmMenu = new UIMenu("EXIT DEVLEVEL?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            _pausebox = new UIDivider(true, 0.8f);
            _pausebox.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_pauseGroup), UIAlign.Left), true);
            _pausebox.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
            _pausebox.leftSection.Add(new UIText("", Color.White), true);
            _pausebox.leftSection.Add(new UIMenuItem("|DGRED|EXIT DEVLEVEL", new UIMenuActionOpenMenu(_pauseMenu, _confirmMenu), UIAlign.Left), true);
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

            _pauseGroup.isPauseMenu = true;
            _pauseGroup.Close();
            Add(_pauseGroup);
          
            Music.Play("Arcade");
            //_exitDoor = new MetroidDoor(-192f, 320f);
            //Add(_exitDoor);
           // _followCam.hardLimitLeft = -192f;
        }

        public override void Terminate()
        {
        }
        public Vec2 SpawnPosition = Vec2.Zero;
        private bool sign;
        private MetroidDoor _exitDoor;
        private bool _enteringCameraUpdated;
        private bool _entering;

        public override void Update()
        {
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
            if (spawnedducks.Count > 0)
            {
                _duck = spawnedducks[0];
            }
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
            for (int i = 0; i < spawnedducks.Count; i++)
            {
                Duck duck = spawnedducks[i];
                if (duck != null && duck.dead)
                {
                    followCam.Clear();
                    Remove(duck);
                    followCam.Remove(duck);
                    duck = new Duck(SpawnPosition.x, SpawnPosition.y, _duck.profile);
                    spawnedducks[i] = duck;
                    followCam.Add(duck);
                    Add(duck);
                    //HUD.AddInputChangeDisplay(" Cmon Now That Was Dumb, Dont You Agree? ");
                }
                for (int index = 0; index < 200; ++index)
                    _followCam.Update();
            }
            if (_pendingSpawns != null && _pendingSpawns.Count > 0)
            {
                Duck pendingSpawn = _pendingSpawns[0];
                AddThing(pendingSpawn);
                _pendingSpawns.RemoveAt(0);

                _duck = pendingSpawn;
                if (!_enteringCameraUpdated)
                {
                    _enteringCameraUpdated = true;
                    for (int index = 0; index < 200; ++index)
                        _followCam.Update();
                }
            }
            if (!_quitting)
            {
                if (Input.Pressed(Triggers.Start))
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
                //if (editor != null)
                //{
                //    current = editor;
                //    return;
                //}
                current = new TitleScreen();
                return;
            }
            if (_paused)
                return;
            Layer.Game.fade = Lerp.Float(Layer.Game.fade, 1f, 0.08f);
            Layer.Background.fade = Lerp.Float(Layer.Game.fade, 1f, 0.08f);
           // _hud.alpha = Lerp.Float(_hud.alpha, 0f, 0.08f);
        }

        public override void PostDrawLayer(Layer layer)
        {
            base.PostDrawLayer(layer);
        }
    }
}