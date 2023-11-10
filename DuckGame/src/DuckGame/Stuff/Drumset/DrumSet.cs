namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    [BaggedProperty("canSpawn", false)]
    public class DrumSet : Holdable, IPlatform
    {
        private BassDrum _bass;
        private Snare _snare;
        private HiHat _hat;
        private LowTom _lowTom;
        private CrashCymbal _crash;
        private MediumTom _medTom;
        private HighTom _highTom;
        public StateBinding _netBassDrumBinding = new NetSoundBinding(nameof(_netBassDrum));
        public StateBinding _netSnareBinding = new NetSoundBinding(nameof(_netSnare));
        public StateBinding _netHatBinding = new NetSoundBinding(nameof(_netHat));
        public StateBinding _netHatAlternateBinding = new NetSoundBinding(nameof(_netHatAlternate));
        public StateBinding _netLowTomBinding = new NetSoundBinding(nameof(_netLowTom));
        public StateBinding _netMediumTomBinding = new NetSoundBinding(nameof(_netMediumTom));
        public StateBinding _netHighTomBinding = new NetSoundBinding(nameof(_netHighTom));
        public StateBinding _netCrashBinding = new NetSoundBinding(nameof(_netCrash));
        public StateBinding _netThrowStickBinding = new NetSoundBinding(nameof(_netThrowStick));
        public NetSoundEffect _netBassDrum = new NetSoundEffect();
        public NetSoundEffect _netSnare = new NetSoundEffect();
        public NetSoundEffect _netHat = new NetSoundEffect();
        public NetSoundEffect _netHatAlternate = new NetSoundEffect();
        public NetSoundEffect _netLowTom = new NetSoundEffect();
        public NetSoundEffect _netMediumTom = new NetSoundEffect();
        public NetSoundEffect _netHighTom = new NetSoundEffect();
        public NetSoundEffect _netCrash = new NetSoundEffect();
        public NetSoundEffect _netThrowStick = new NetSoundEffect();
        private int hits;
        private int tick = 15;
        private int hitsSinceThrow;

        public override Vec2 netPosition
        {
            get => position;
            set => position = value;
        }

        public DrumSet(float xpos, float ypos)
          : base(xpos, ypos)
        {
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -11f);
            collisionSize = new Vec2(16f, 14f);
            depth = (Depth)0.5f;
            thickness = 0f;
            weight = 7f;
            _editorIcon = new Sprite("drumsIcon");
            _holdOffset = new Vec2(1f, 7f);
            flammable = 0.3f;
            collideSounds.Add("rockHitGround2");
            handOffset = new Vec2(0f, -9999f);
            holsterable = false;
            tapeable = false;
            hugWalls = WallHug.Floor;
            editorTooltip = "Stress got you down? It's time to jam!";
        }

        public void ThrowStick()
        {
            if (Rando.Float(1f) >= 0.5)
                Level.Add(new DrumStick(x - 5f, y - 8f));
            else
                Level.Add(new DrumStick(x + 5f, y - 8f));
        }

        public override void Initialize()
        {
            _bass = new BassDrum(x, y);
            Level.Add(_bass);
            _snare = new Snare(x, y);
            Level.Add(_snare);
            _hat = new HiHat(x, y);
            Level.Add(_hat);
            _lowTom = new LowTom(x, y);
            Level.Add(_lowTom);
            _crash = new CrashCymbal(x, y);
            Level.Add(_crash);
            _medTom = new MediumTom(x, y);
            Level.Add(_medTom);
            _highTom = new HighTom(x, y);
            Level.Add(_highTom);
            _bass.position = position;
            _bass.depth = depth + 1;
            _snare.position = position + new Vec2(10f, -7f);
            _snare.depth = depth;
            _hat.depth = depth - 1;
            _hat.position = position + new Vec2(13f, -11f);
            _lowTom.depth = depth - 1;
            _lowTom.position = position + new Vec2(-9f, -5f);
            _crash.depth = depth;
            _crash.position = position + new Vec2(-15f, -15f);
            _medTom.depth = depth + 3;
            _medTom.position = position + new Vec2(-8f, -12f);
            _highTom.depth = depth + 3;
            _highTom.position = position + new Vec2(7f, -12f);
            _netBassDrum.function = new NetSoundEffect.Function(_bass.Hit);
            _netSnare.function = new NetSoundEffect.Function(_snare.Hit);
            _netHat.function = new NetSoundEffect.Function(_hat.Hit);
            _netHatAlternate.function = new NetSoundEffect.Function(_hat.AlternateHit);
            _netLowTom.function = new NetSoundEffect.Function(_lowTom.Hit);
            _netMediumTom.function = new NetSoundEffect.Function(_medTom.Hit);
            _netHighTom.function = new NetSoundEffect.Function(_highTom.Hit);
            _netCrash.function = new NetSoundEffect.Function(_crash.Hit);
            _netThrowStick.function = new NetSoundEffect.Function(ThrowStick);
        }

        public override void Terminate()
        {
            Level.Remove(_bass);
            Level.Remove(_snare);
            Level.Remove(_hat);
            Level.Remove(_lowTom);
            Level.Remove(_medTom);
            Level.Remove(_highTom);
            Level.Remove(_crash);
        }

        public override void Update()
        {
            --tick;
            if (tick <= 0)
            {
                tick = 15;
                --hits;
            }
            if (hits < 0)
                hits = 0;
            if (owner == null || held)
                depth = (Depth)0.5f;
            if (owner != null && held)
            {
                owner.vSpeed = 0f;
                owner.hSpeed = 0f;
                if (isServerForObject)
                {
                    int hits1 = hits;
                    if (duck.inputProfile.Pressed(Triggers.Up))
                    {
                        if (Network.isActive)
                            _netCrash.Play();
                        else
                            _crash.Hit();
                        ++hits;
                    }
                    if (duck.inputProfile.Pressed(Triggers.Shoot))
                    {
                        if (Network.isActive)
                            _netSnare.Play();
                        else
                            _snare.Hit();
                        ++hits;
                    }
                    if (duck.inputProfile.Pressed(Triggers.Right))
                    {
                        if (Network.isActive)
                            _netHighTom.Play();
                        else
                            _highTom.Hit();
                        ++hits;
                    }
                    if (duck.inputProfile.Pressed(Triggers.Down))
                    {
                        if (Network.isActive)
                            _netMediumTom.Play();
                        else
                            _medTom.Hit();
                        ++hits;
                    }
                    if (duck.inputProfile.Pressed(Triggers.Left))
                    {
                        if (Network.isActive)
                            _netLowTom.Play();
                        else
                            _lowTom.Hit();
                        ++hits;
                    }
                    if (duck.inputProfile.Pressed(Triggers.Jump))
                    {
                        if (Network.isActive)
                            _netHat.Play();
                        else
                            _hat.Hit();
                        ++hits;
                    }
                    if (duck.inputProfile.Pressed(Triggers.LeftTrigger))
                    {
                        if (Network.isActive)
                            _netHat.Play();
                        else
                            _hat.AlternateHit();
                        ++hits;
                    }
                    if (duck.inputProfile.Pressed(Triggers.Ragdoll) || duck.inputProfile.Pressed(Triggers.Strafe))
                    {
                        if (Network.isActive)
                            _netBassDrum.Play();
                        else
                            _bass.Hit();
                        ++hits;
                    }
                    int hits2 = hits;
                    if (hits1 != hits2)
                    {
                        if (duck != null)
                            RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
                        ++hitsSinceThrow;
                        if (hits * 0.02f > Rando.Float(1f) && Rando.Float(1f) > 0.95f && hitsSinceThrow > 10)
                        {
                            if (Network.isActive)
                                _netThrowStick.Play();
                            else
                                ThrowStick();
                            hitsSinceThrow = 0;
                        }
                    }
                }
            }
            _bass.position = position;
            _bass.depth = depth + 1;
            _snare.position = position + new Vec2(10f, -7f);
            _snare.depth = depth;
            _hat.depth = depth - 1;
            _hat.position = position + new Vec2(13f, -11f);
            _lowTom.depth = depth - 1;
            _lowTom.position = position + new Vec2(-9f, -5f);
            _crash.depth = depth;
            _crash.position = position + new Vec2(-15f, -15f);
            _medTom.depth = depth + 3;
            _medTom.position = position + new Vec2(-8f, -12f);
            _highTom.depth = depth + 3;
            _highTom.position = position + new Vec2(7f, -12f);
        }
    }
}
