using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class DGRDevHall : XMLLevel
    {
        public DGRDevHall(string name) : base(name)
        {
            _followCam = new FollowCam
            {
                lerpMult = 1f,
                startCentered = false
            };
            camera = _followCam;
        }
        private List<Duck> _pendingSpawns;
        public Vec2 SpawnPosition;
        private UIDivider _pausebox;
        private UIMenu _confirmMenu;
        private MenuBoolean _quit = new MenuBoolean();
        private UIMenu _pauseMenu;
        private UIComponent _pauseGroup;
        public bool _paused;
        public override void Initialize()
        {
            TeamSelect2.DefaultSettings();
            base.Initialize();
            _pendingSpawns = new Deathmatch(this).SpawnPlayers(false);
            foreach (Duck pendingSpawn in _pendingSpawns)
            {
                SpawnPosition = pendingSpawn.position;
                followCam.Add(pendingSpawn);
                First<ArcadeHatConsole>()?.MakeHatSelector(pendingSpawn);
            }
            followCam.Adjust();

            _pauseGroup = new UIComponent(Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 0f, 0f);
            _pauseMenu = new UIMenu("@LWING@ARCADE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@CLOSE  @SELECT@SELECT");
            _confirmMenu = new UIMenu("EXIT DEV HALL?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 160f, conString: "@CANCEL@BACK  @SELECT@SELECT");
            _pausebox = new UIDivider(true, 0.8f);
            _pausebox.leftSection.Add(new UIMenuItem("RESUME", new UIMenuActionCloseMenu(_pauseGroup), UIAlign.Left), true);
            _pausebox.leftSection.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(_pauseMenu, Options.optionsMenu), UIAlign.Left), true);
            _pausebox.leftSection.Add(new UIText("", Color.White), true);
            _pausebox.leftSection.Add(new UIMenuItem("|DGRED|EXIT DEV HALL", new UIMenuActionOpenMenu(_pauseMenu, _confirmMenu), UIAlign.Left), true);
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
        }
        public Duck _duck;
        public bool _entering = true;
        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.Foreground && currentDev.DisplayName != "tmob03")
            {
                string[] st = Extensions.SplitByLength(currentDev.Info, 40);
                int its = 0;

                Vec2 vec = drawPos + new Vec2(28, -32);

                foreach (string s in st)
                {
                    Graphics.DrawString(s, vec + new Vec2(3, its * 6 + 3), Color.White, 1, null, 0.5f);
                    
                    its++;
                }
                Graphics.DrawRect(vec + new Vec2(2), vec + new Vec2(164, its * 6 + 2), Color.Black, 0.99f);
                Graphics.DrawRect(vec, vec + new Vec2(166, its * 6 + 4), Color.White, 0.98f);
            }
            base.PostDrawLayer(layer);
        }
        public Vec2 drawPos;
        public DGRebuiltDeveloper currentDev  = DGRDevs.Tmob03;
        public override void Update()
        {
            if (_quit != null && _duck != null)
            {
                if (_quit.value)
                {
                    Graphics.fade = Lerp.Float(Graphics.fade, 0, 0.05f);
                    if (Graphics.fade <= 0)
                    {
                        current = new TitleScreen(_duck.profile);
                    }
                }
                else
                {
                    Music.Resume();
                    _duck.immobilized = false;
                    simulatePhysics = true;
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
                    }
                }
            }
            Duck d = First<Duck>();
            if (d != null && d.holdObject != null)
            {
                if (d.holdObject is NiK0Gun) currentDev = DGRDevs.NiK0;
                else if (d.holdObject is CollinGun) currentDev = DGRDevs.Collin;
                else if (d.holdObject is DanGun) currentDev = DGRDevs.Dan;
                else if (d.holdObject is FirebreakGun) currentDev = DGRDevs.Firebreak;
                else if (d.holdObject is LutalliGun) currentDev = DGRDevs.Lutalli;
                else if (d.holdObject is othello7Gun) currentDev = DGRDevs.Othello7;
                else if (d.holdObject is HyeveGun) currentDev = DGRDevs.Hyeve;
                else if (d.holdObject is KlofGun) currentDev = DGRDevs.Klof44;
                else if (d.holdObject is MoroGun) currentDev = DGRDevs.Moro;
                else currentDev = DGRDevs.Tmob03; //use tmob in replacement of null
                drawPos = d.holdObject.position;
            }
            else currentDev = DGRDevs.Tmob03;
            if (_entering)
            {
                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.05f);
                if (Graphics.fade > 0.99f)
                {
                    _entering = false;
                    Graphics.fade = 1f;
                }
            }
            if (_pendingSpawns != null && _pendingSpawns.Count > 0)
            {
                Duck pendingSpawn = _pendingSpawns[0];
                AddThing(pendingSpawn);
                _pendingSpawns.RemoveAt(0);
                _duck = pendingSpawn;
            }
            base.Update();
        }
        private FollowCam _followCam;
        public FollowCam followCam => _followCam;
    }
}
