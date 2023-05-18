// Decompiled with JetBrains decompiler
// Type: DuckGame.Magnum
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [ClientOnly]
    //[EditorGroup("Rebuilt|Wump|Pistols")]
    public class WumpMagnum : Gun
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

        public WumpMagnum(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 6;
            _ammoType = new ATWumpMagnum();
            _type = "gun";
            graphic = new Sprite("wumpmagnum");
            center = new Vec2(12f, 6f);
            collisionOffset = new Vec2(-8f, -6f);
            collisionSize = new Vec2(16f, 10f);
            _barrelOffsetTL = new Vec2(22f, 3f);
            _fireSound = "magnum";
            _kickForce = 3f;
            _fireRumble = RumbleIntensity.Light;
            _holdOffset = new Vec2(1f, 2f);
            handOffset = new Vec2(0f, 1f);
            _editorName = "Wump Magnum";
            editorTooltip = "A powerful round that can withstand colliding once with the possibility of hitting a target, be careful with this one.";
        }

        public override void Update()
        {
            base.Update();
            _angleOffset = owner == null ? 0f : (offDir >= 0 ? -Maths.DegToRad(rise * 65f) : -Maths.DegToRad((float)(-rise * 65.0)));
            if (rise > 0.0)
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
            if (ammo <= 0 || rise >= 1.2f)
                return;
            rise += 0.6f;
        }
    }
}
