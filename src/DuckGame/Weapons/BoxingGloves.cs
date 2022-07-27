// Decompiled with JetBrains decompiler
// Type: DuckGame.BoxingGloves
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            get => base.angle + (this._swing + this._hold) * offDir;
            set => this._angle = value;
        }

        public BoxingGloves(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";
            this.graphic = new Sprite("boxingGlove");
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-4f, -4f);
            this.collisionSize = new Vec2(8f, 8f);
            this._barrelOffsetTL = new Vec2(16f, 7f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 3f;
            this._holdOffset = new Vec2(-4f, 4f);
            this.weight = 0.9f;
            this.physicsMaterial = PhysicsMaterial.Paper;
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
