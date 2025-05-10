using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class DeathmatchLevel : XMLLevel, IHaveAVirtualTransition
    {
        protected static bool _started = false;
        public static bool playedGame = false;
        protected FollowCam _followCam;
        private Vec2 _p1;
        private Vec2 _p2;
        protected List<Duck> _pendingSpawns;
        protected float _waitSpawn = 1.8f;
        private float _waitFade = 1f;
        private float _waitAfterSpawn;
        private int _waitAfterSpawnDings;
        private BitmapFont _font;
        private float _fontFade = 1f;
        protected Deathmatch _deathmatch;
        public static bool firstDead = false;
        private bool _deathmatchStarted;
        private bool _didPlay;
        public static bool newLevel = true;
        private bool didStart;

        public static bool started
        {
            get => _started;
            set => _started = value;
        }

        public FollowCam followCam => _followCam;

        public DeathmatchLevel(string level)
          : base(level)
        {
            _followCam = new FollowCam
            {
                lerpMult = 1.2f
            };
            camera = _followCam;
            _started = false;
        }

        public override void Terminate()
        {
            base.Terminate();
            foreach (Profile profile in Profiles.active)
                profile.duck = null;
        }
        public int spawnAmount;
        public override void Initialize()
        {
            firstDead = false;
            playedGame = true;
            foreach (Profile profile in Profiles.active)
                profile.duck = null;
            _font = new BitmapFont("biosFont", 8);
            base.Initialize();
            if (!Network.isActive)
                StartDeathmatch();
            _deathmatch = new Deathmatch(this);
            AddThing(_deathmatch);
            _pendingSpawns = _deathmatch.SpawnPlayers(true);
            _pendingSpawns = _pendingSpawns.OrderBy(sp => sp.x).ToList();
            spawnAmount = 1;
            if (_pendingSpawns.Count > 8)
            {
                spawnAmount += _pendingSpawns.Count / 8;
            }
            foreach (Thing pendingSpawn in _pendingSpawns)
                followCam.Add(pendingSpawn);
            followCam.Adjust();
            _things.RefreshState();
            _p1 = new Vec2(9999f, -9999f);
            _p2 = Vec2.Zero;
            int num = 0;
            foreach (Duck duck in things[typeof(Duck)])
            {
                if (duck.x < _p1.x)
                    _p1 = duck.position;
                _p2 += duck.position;
                ++num;
            }
            _p2 /= num;
        }

        public void StartDeathmatch()
        {
            _deathmatchStarted = true;
            Music.LoadAlternateSong(Music.RandomTrack("InGame", Music.currentSong));
            Music.CancelLooping();
        }

        public override void Update()
        {
            if (!_deathmatchStarted)
                return;
            if (!didStart)
            {
                Graphics.fade = 1f;
                didStart = true;
            }
            if (_deathmatch != null && Music.finished)
            {
                if (_didPlay)
                {
                    Music.Play(Music.currentSong, false);
                }
                else
                {
                    if (Music.pendingSong != null)
                        Music.SwitchSongs();
                    else
                        _deathmatch.PlayMusic();
                    _didPlay = true;
                }
            }
            _waitFade -= 0.04f;
            if (_waitFade > 0f)
                return;
            _waitSpawn -= 0.06f;
            if (_waitSpawn > 0f)
                return;
            if (_pendingSpawns != null && _pendingSpawns.Count > 0)
            {
                _waitSpawn = 1.1f;
                if (_pendingSpawns.Count == 1)
                    _waitSpawn = 2f;
                int loops = 1;
                if (spawnAmount > 1)
                {
                    _waitSpawn -= spawnAmount / 10f;
                    if (_pendingSpawns.Count > spawnAmount) loops = spawnAmount;
                    else loops = _pendingSpawns.Count;
                }
                for (int i = 0; i < loops; i++)
                {
                    Duck pendingSpawn = _pendingSpawns[0];
                    pendingSpawn.visible = true;
                    AddThing(pendingSpawn);
                    _pendingSpawns.RemoveAt(0);
                    if (Network.isServer && Network.isActive)
                        Send.Message(new NMSpawnDuck(pendingSpawn.netProfileIndex));

                    Vec3 color = pendingSpawn.profile.persona.color;
                    Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, 0f, new Color((int)color.x, (int)color.y, (int)color.z), 32f));
                    Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, -4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                    Add(new SpawnLine(pendingSpawn.x, pendingSpawn.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
                    Add(new SpawnAimer(pendingSpawn.x, pendingSpawn.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), pendingSpawn.persona, 4f));
                    SFX.Play("pullPin", 0.7f);
                    if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Present) || TeamSelect2.Enabled("WINPRES") && Deathmatch.lastWinners.Contains(pendingSpawn.profile))
                    {
                        Present h = new Present(pendingSpawn.x, pendingSpawn.y);
                        Add(h);
                        pendingSpawn.GiveHoldable(h);
                    }
                    if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Jetpack) || TeamSelect2.Enabled("JETTY"))
                    {
                        Jetpack e = new Jetpack(pendingSpawn.x, pendingSpawn.y);
                        Add(e);
                        pendingSpawn.Equip(e);
                    }
                    if (TeamSelect2.Enabled("HELMY"))
                    {
                        Helmet e = new Helmet(pendingSpawn.x, pendingSpawn.y);
                        Add(e);
                        pendingSpawn.Equip(e);
                    }
                    if (TeamSelect2.Enabled("SHOESTAR"))
                    {
                        Boots e = new Boots(pendingSpawn.x, pendingSpawn.y);
                        Add(e);
                        pendingSpawn.Equip(e);
                    }
                    if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Armor))
                    {
                        Helmet e1 = new Helmet(pendingSpawn.x, pendingSpawn.y);
                        Add(e1);
                        pendingSpawn.Equip(e1);
                        ChestPlate e2 = new ChestPlate(pendingSpawn.x, pendingSpawn.y);
                        Add(e2);
                        pendingSpawn.Equip(e2);
                    }
                    if (Party.HasPerk(pendingSpawn.profile, PartyPerks.Pistol))
                    {
                        Pistol h = new Pistol(pendingSpawn.x, pendingSpawn.y);
                        Add(h);
                        pendingSpawn.GiveHoldable(h);
                    }
                    if (!Party.HasPerk(pendingSpawn.profile, PartyPerks.NetGun))
                        return;
                    NetGun h1 = new NetGun(pendingSpawn.x, pendingSpawn.y);
                    Add(h1);
                    pendingSpawn.GiveHoldable(h1);
                }
            }
            else if (!_started)
            {
                _waitAfterSpawn -= 0.05f;
                if (_waitAfterSpawn > 0f)
                    return;
                if (Network.isServer && Network.isActive && _waitAfterSpawnDings == 0)
                    Send.Message(new NMGetReady());
                ++_waitAfterSpawnDings;
                if (_waitAfterSpawnDings > 2)
                {
                    Party.Clear();
                    _started = true;
                    SFX.Play("ding");
                    Event.Log(new RoundStartEvent());
                }
                else SFX.Play("preStartDing");
                _waitSpawn = 1.1f;
            }
            else
            {
                _fontFade -= 0.1f;
                if (_fontFade >= 0f)
                    return;
                _fontFade = 0f;
            }
        }

        public override void PostDrawLayer(Layer layer)
        {
            if (layer == Layer.HUD && _waitAfterSpawnDings > 0 && _fontFade > 0.01f)
            {
                _font.scale = new Vec2(2f, 2f);
                _font.alpha = _fontFade;
                string text = "GET";
                if (_waitAfterSpawnDings == 2)
                    text = "READY";
                else if (_waitAfterSpawnDings == 3)
                    text = "";
                float width = _font.GetWidth(text);
                _font.Draw(text, (Layer.HUD.camera.width / 2f - width / 2f), (Layer.HUD.camera.height / 2f - _font.height / 2f), Color.White);
            }
            base.PostDrawLayer(layer);
        }
    }
}
