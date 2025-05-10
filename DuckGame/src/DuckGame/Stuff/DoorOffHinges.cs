namespace DuckGame
{
    public class DoorOffHinges : PhysicsObject
    {
        public StateBinding _throwSpinBinding = new StateBinding(nameof(_throwSpin));
        public StateBinding _secondaryBinding = new StateBinding(nameof(_secondaryFrame));
        public bool _secondaryFrame;
        public bool _wasSecondaryFrame;
        public float _throwSpin;
        private bool sounded;

        public DoorOffHinges(float xpos, float ypos, bool secondaryFrame)
          : base(xpos, ypos)
        {
            _secondaryFrame = secondaryFrame;
            _collisionSize = new Vec2(8f, 8f);
            _collisionOffset = new Vec2(-4f, -6f);
            center = new Vec2(16f, 16f);
            collideSounds.Add("rockHitGround");
            weight = 2f;
        }

        public override void Initialize()
        {
            RefreshSprite();
            base.Initialize();
        }

        public void MakeEffects()
        {
            if (sounded)
                return;
            SFX.Play("doorBreak");
            if (DGRSettings.S_ParticleMultiplier != 0)
            {
                Level.Add(SmallSmoke.New(x, y + 2f));
                Level.Add(SmallSmoke.New(x, y - 16f));
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 8; ++index)
                {
                    WoodDebris woodDebris = WoodDebris.New(x - 8f + Rando.Float(16f), y - 8f + Rando.Float(16f));
                    woodDebris.hSpeed = (Rando.Float(1f) > 0.5 ? 1f : -1f) * Rando.Float(3f);
                    woodDebris.vSpeed = -Rando.Float(1f);
                    Level.Add(woodDebris);
                }

            }
            sounded = true;
        }

        private void RefreshSprite()
        {
            graphic = new SpriteMap(_secondaryFrame ? "flimsyDoorDamaged" : "doorFucked", 32, 32);
            _wasSecondaryFrame = _secondaryFrame;
        }

        public override void Update()
        {
            if (_secondaryFrame != _wasSecondaryFrame)
                RefreshSprite();
            if (Network.isActive && !sounded && visible)
                MakeEffects();
            angleDegrees = _throwSpin;
            center = new Vec2(16f, 16f);
            _throwSpin %= 360f;
            _throwSpin = offDir <= 0 ? Lerp.Float(_throwSpin, -90f, 12f) : Lerp.Float(_throwSpin, 90f, 12f);
            base.Update();
        }
    }
}
