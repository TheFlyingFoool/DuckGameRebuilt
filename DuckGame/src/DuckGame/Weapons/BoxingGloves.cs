namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    public class BoxingGloves : Gun
    {
        private float _swing;
        private float _hold;

        public override float angle
        {
            get => base.angle + (_swing + _hold) * offDir;
            set => _angle = value;
        }

        public BoxingGloves(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 4;
            _ammoType = new ATLaser
            {
                range = 170f,
                accuracy = 0.8f
            };
            _type = "gun";
            graphic = new Sprite("boxingGlove");
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -4f);
            collisionSize = new Vec2(8f, 8f);
            _barrelOffsetTL = new Vec2(16f, 7f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 3f;
            _holdOffset = new Vec2(-4f, 4f);
            weight = 0.9f;
            physicsMaterial = PhysicsMaterial.Paper;
        }

        public override void Initialize() => base.Initialize();

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            SFX.Play("ting");
            return base.Hit(bullet, hitPos);
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
        }

        public override void Fire()
        {
        }
    }
}
