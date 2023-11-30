namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isFatal", false)]
    [BaggedProperty("isInDemo", true)]
    public class DemoBlaster : Gun
    {
        private FluidStream _stream;
        private ConstantSound _sound;
        private int _wait;

        public DemoBlaster(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 9;
            _ammoType = new AT9mm();
            _type = "gun";
            graphic = new Sprite("demoBlaster");
            center = new Vec2(18f, 8f);
            collisionOffset = new Vec2(-16f, -8f);
            collisionSize = new Vec2(32f, 15f);
            _barrelOffsetTL = new Vec2(37f, 7f);
            _fireSound = "pistolFire";
            _kickForce = 3f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-1f, -4f);
            weight = 8f;
            loseAccuracy = 0.1f;
            maxAccuracyLost = 0.6f;
            _bio = "";
            _editorName = "Demo Blaster";
            physicsMaterial = PhysicsMaterial.Metal;
            _stream = new FluidStream(x, y, new Vec2(1f, 0f), 1f);
            isFatal = false;
        }

        public override void Initialize()
        {
            _sound = new ConstantSound("demoBlaster");
            Level.Add(_stream);
        }

        public override void Terminate() => Level.Remove(_stream);

        public override void Update()
        {
            _sound.lerpVolume = _triggerHeld ? 1f : 0f;
            base.Update();
        }

        public override void OnPressAction()
        {
        }

        public override void OnHoldAction()
        {
            _wait++;
            if (_wait == 3)
            {
                _stream.sprayAngle = barrelVector * 2f;
                _stream.position = barrelPosition;
                FluidData dat = Fluid.Poo;
                dat.amount = 0.01f;
                _stream.Feed(dat);
                _wait = 0;
            }
        }
    }
}
