// Decompiled with JetBrains decompiler
// Type: DuckGame.BoxingGloves
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            _ammoType = new ATLaser();
            _ammoType.range = 170f;
            _ammoType.accuracy = 0.8f;
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
