// Decompiled with JetBrains decompiler
// Type: DuckGame.DrumSet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            get => this.position;
            set => this.position = value;
        }

        public DrumSet(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -11f);
            this.collisionSize = new Vec2(16f, 14f);
            this.depth = (Depth)0.5f;
            this.thickness = 0f;
            this.weight = 7f;
            this._editorIcon = new Sprite("drumsIcon");
            this._holdOffset = new Vec2(1f, 7f);
            this.flammable = 0.3f;
            this.collideSounds.Add("rockHitGround2");
            this.handOffset = new Vec2(0f, -9999f);
            this.holsterable = false;
            this.tapeable = false;
            this.hugWalls = WallHug.Floor;
            this.editorTooltip = "Stress got you down? It's time to jam!";
        }

        public void ThrowStick()
        {
            if (Rando.Float(1f) >= 0.5)
                Level.Add(new DrumStick(this.x - 5f, this.y - 8f));
            else
                Level.Add(new DrumStick(this.x + 5f, this.y - 8f));
        }

        public override void Initialize()
        {
            this._bass = new BassDrum(this.x, this.y);
            Level.Add(_bass);
            this._snare = new Snare(this.x, this.y);
            Level.Add(_snare);
            this._hat = new HiHat(this.x, this.y);
            Level.Add(_hat);
            this._lowTom = new LowTom(this.x, this.y);
            Level.Add(_lowTom);
            this._crash = new CrashCymbal(this.x, this.y);
            Level.Add(_crash);
            this._medTom = new MediumTom(this.x, this.y);
            Level.Add(_medTom);
            this._highTom = new HighTom(this.x, this.y);
            Level.Add(_highTom);
            this._bass.position = this.position;
            this._bass.depth = this.depth + 1;
            this._snare.position = this.position + new Vec2(10f, -7f);
            this._snare.depth = this.depth;
            this._hat.depth = this.depth - 1;
            this._hat.position = this.position + new Vec2(13f, -11f);
            this._lowTom.depth = this.depth - 1;
            this._lowTom.position = this.position + new Vec2(-9f, -5f);
            this._crash.depth = this.depth;
            this._crash.position = this.position + new Vec2(-15f, -15f);
            this._medTom.depth = this.depth + 3;
            this._medTom.position = this.position + new Vec2(-8f, -12f);
            this._highTom.depth = this.depth + 3;
            this._highTom.position = this.position + new Vec2(7f, -12f);
            this._netBassDrum.function = new NetSoundEffect.Function(_bass.Hit);
            this._netSnare.function = new NetSoundEffect.Function(_snare.Hit);
            this._netHat.function = new NetSoundEffect.Function(_hat.Hit);
            this._netHatAlternate.function = new NetSoundEffect.Function(_hat.AlternateHit);
            this._netLowTom.function = new NetSoundEffect.Function(_lowTom.Hit);
            this._netMediumTom.function = new NetSoundEffect.Function(_medTom.Hit);
            this._netHighTom.function = new NetSoundEffect.Function(_highTom.Hit);
            this._netCrash.function = new NetSoundEffect.Function(_crash.Hit);
            this._netThrowStick.function = new NetSoundEffect.Function(this.ThrowStick);
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
            --this.tick;
            if (this.tick <= 0)
            {
                this.tick = 15;
                --this.hits;
            }
            if (this.hits < 0)
                this.hits = 0;
            if (this.owner == null || this.held)
                this.depth = (Depth)0.5f;
            if (this.owner != null && this.held)
            {
                this.owner.vSpeed = 0f;
                this.owner.hSpeed = 0f;
                if (this.isServerForObject)
                {
                    int hits1 = this.hits;
                    if (this.duck.inputProfile.Pressed("UP"))
                    {
                        if (Network.isActive)
                            this._netCrash.Play();
                        else
                            this._crash.Hit();
                        ++this.hits;
                    }
                    if (this.duck.inputProfile.Pressed("SHOOT"))
                    {
                        if (Network.isActive)
                            this._netSnare.Play();
                        else
                            this._snare.Hit();
                        ++this.hits;
                    }
                    if (this.duck.inputProfile.Pressed("RIGHT"))
                    {
                        if (Network.isActive)
                            this._netHighTom.Play();
                        else
                            this._highTom.Hit();
                        ++this.hits;
                    }
                    if (this.duck.inputProfile.Pressed("DOWN"))
                    {
                        if (Network.isActive)
                            this._netMediumTom.Play();
                        else
                            this._medTom.Hit();
                        ++this.hits;
                    }
                    if (this.duck.inputProfile.Pressed("LEFT"))
                    {
                        if (Network.isActive)
                            this._netLowTom.Play();
                        else
                            this._lowTom.Hit();
                        ++this.hits;
                    }
                    if (this.duck.inputProfile.Pressed("JUMP"))
                    {
                        if (Network.isActive)
                            this._netHat.Play();
                        else
                            this._hat.Hit();
                        ++this.hits;
                    }
                    if (this.duck.inputProfile.Pressed("LTRIGGER"))
                    {
                        if (Network.isActive)
                            this._netHat.Play();
                        else
                            this._hat.AlternateHit();
                        ++this.hits;
                    }
                    if (this.duck.inputProfile.Pressed("RAGDOLL") || this.duck.inputProfile.Pressed("STRAFE"))
                    {
                        if (Network.isActive)
                            this._netBassDrum.Play();
                        else
                            this._bass.Hit();
                        ++this.hits;
                    }
                    int hits2 = this.hits;
                    if (hits1 != hits2)
                    {
                        if (this.duck != null)
                            RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
                        ++this.hitsSinceThrow;
                        if (hits * 0.02f > Rando.Float(1f) && Rando.Float(1f) > 0.95f && this.hitsSinceThrow > 10)
                        {
                            if (Network.isActive)
                                this._netThrowStick.Play();
                            else
                                this.ThrowStick();
                            this.hitsSinceThrow = 0;
                        }
                    }
                }
            }
            this._bass.position = this.position;
            this._bass.depth = this.depth + 1;
            this._snare.position = this.position + new Vec2(10f, -7f);
            this._snare.depth = this.depth;
            this._hat.depth = this.depth - 1;
            this._hat.position = this.position + new Vec2(13f, -11f);
            this._lowTom.depth = this.depth - 1;
            this._lowTom.position = this.position + new Vec2(-9f, -5f);
            this._crash.depth = this.depth;
            this._crash.position = this.position + new Vec2(-15f, -15f);
            this._medTom.depth = this.depth + 3;
            this._medTom.position = this.position + new Vec2(-8f, -12f);
            this._highTom.depth = this.depth + 3;
            this._highTom.position = this.position + new Vec2(7f, -12f);
        }
    }
}
