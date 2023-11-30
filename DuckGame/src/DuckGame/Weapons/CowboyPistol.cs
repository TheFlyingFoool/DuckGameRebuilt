namespace DuckGame
{
    [EditorGroup("Guns|Pistols", EditorItemType.PowerUser)]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class CowboyPistol : Gun
    {
        private float rise;
        private float _angleOffset;

        public override float angle
        {
            get
            {
                if (_raised || duck == null)
                    return base.angle;
                Vec2 p2 = duck.inputProfile.rightStick;
                if (p2.length < 0.1f)
                {
                    p2 = Vec2.Zero;
                    return base.angle;
                }
                return offDir > 0 ? Maths.DegToRad(Maths.PointDirection(Vec2.Zero, p2)) : Maths.DegToRad(Maths.PointDirection(Vec2.Zero, p2) + 180f);
            }
            set => _angle = value;
        }

        public CowboyPistol(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 6;
            _ammoType = new ATMagnum();
            _type = "gun";
            graphic = new Sprite("cowboyPistol");
            center = new Vec2(6f, 7f);
            collisionOffset = new Vec2(-5f, -7f);
            collisionSize = new Vec2(18f, 11f);
            _barrelOffsetTL = new Vec2(21f, 3f);
            _fireSound = "magnum";
            _kickForce = 3f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-2f, 1f);
            _bio = "Standard issue .44 Magnum. Pretty great for killing things, really great for killing things that are trying to hide. Watch the kick, unless you're trying to shoot the ceiling.";
            _editorName = "Cowboy Pistol";
        }

        public override void Update()
        {
            base.Update();
            _angleOffset = owner == null ? 0f : (offDir >= 0 ? -Maths.DegToRad(rise * 65f) : -Maths.DegToRad((-rise * 65f)));
            if (rise > 0f)
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
