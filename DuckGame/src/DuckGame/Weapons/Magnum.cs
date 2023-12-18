namespace DuckGame
{
    [EditorGroup("Guns|Pistols")]
    [BaggedProperty("isInDemo", true)]
    public class Magnum : Gun
    {
        public StateBinding _angleOffsetBinding = new StateBinding(nameof(_angleOffset));
        public StateBinding _riseBinding = new StateBinding(nameof(rise));
        public float rise;
        public float _angleOffset;

        public override float angle
        {
            get => base.angle + _angleOffset;
            set => _angle = value;
        }

        public Magnum(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 6;
            _ammoType = new ATMagnum();
            _type = "gun";
            graphic = new Sprite("magnum");
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -6f);
            collisionSize = new Vec2(16f, 10f);
            _barrelOffsetTL = new Vec2(25f, 12f);
            _fireSound = "magnum";
            _kickForce = 3f;
            _fireRumble = RumbleIntensity.Light;
            _holdOffset = new Vec2(1f, 2f);
            handOffset = new Vec2(0f, 1f);
            _bio = "Standard issue .44 Magnum. Pretty great for killing things, really great for killing things that are trying to hide. Watch the kick, unless you're trying to shoot the ceiling.";
            _editorName = nameof(Magnum);
            editorTooltip = "Heavy duty pistol that pierces many objects. Cool shades not included.";
        }

        public override void Update()
        {
            base.Update();
            _angleOffset = owner == null ? 0f : (offDir >= 0 ? -Maths.DegToRad(rise * 65f) : -Maths.DegToRad((float)(-rise * 65f)));
            if (rise > 0)
                rise -= 0.013f;
            else
                rise = 0f;
            if (!_raised)
                return;
            _angleOffset = 0f;
        }

        public override void OnPressAction()
        {
            base.OnPressAction();
            if (ammo <= 0 || rise >= 1f)
                return;
            rise += 0.4f;
        }
    }
}
