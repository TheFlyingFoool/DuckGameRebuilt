// Decompiled with JetBrains decompiler
// Type: DuckGame.DartGun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    public class DartGun : Gun
    {
        public StateBinding _burnLifeBinding = new StateBinding(nameof(_burnLife));
        private SpriteMap _sprite;
        public float _burnLife = 1f;
        public float _burnWait;
        public bool burntOut;

        public DartGun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 12;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this.wideBarrel = true;
            this.barrelInsertOffset = new Vec2(-3f, -1f);
            this._type = "gun";
            this._sprite = new SpriteMap("dartgun", 32, 32);
            this.graphic = _sprite;
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(16f, 9f);
            this._barrelOffsetTL = new Vec2(29f, 14f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this.flammable = 0.8f;
            this._barrelAngleOffset = 8f;
            this.physicsMaterial = PhysicsMaterial.Plastic;
            this.editorTooltip = "Shoots like a gun, feels like a gentle breeze.";
            this.isFatal = false;
        }

        public override void Initialize() => base.Initialize();

        public override void UpdateFirePosition(SmallFire f) => f.position = this.Offset(new Vec2(10f, 0.0f));

        public override void UpdateOnFire()
        {
            if (!this.onFire)
                return;
            this._burnWait -= 0.01f;
            if (_burnWait < 0.0)
            {
                Level.Add(SmallFire.New(10f, 0.0f, 0.0f, 0.0f, stick: this, canMultiply: false, firedFrom: this));
                this._burnWait = 1f;
            }
            if (burnt >= 1.0)
                return;
            this.burnt += 1f / 1000f;
        }

        public override void Update()
        {
            if (!this.burntOut && burnt >= 1.0)
            {
                this._sprite.frame = 1;
                Vec2 vec2 = this.Offset(new Vec2(10f, 0.0f));
                Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                this._onFire = false;
                this.flammable = 0.0f;
                this.burntOut = true;
            }
            base.Update();
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            this.onFire = true;
            return true;
        }

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                if (_burnLife <= 0.0)
                {
                    SFX.Play("dartStick", 0.5f, Rando.Float(0.2f) - 0.1f);
                }
                else
                {
                    --this.ammo;
                    SFX.Play("dartGunFire", 0.5f, Rando.Float(0.2f) - 0.1f);
                    this.kick = 1f;
                    if (this.receivingPress || !this.isServerForObject)
                        return;
                    Vec2 vec2 = this.Offset(this.barrelOffset + new Vec2(-8f, 0.0f));
                    float radians = this.barrelAngle + Rando.Float(-0.05f, 0.05f);
                    Dart dart = new Dart(vec2.x, vec2.y, this.owner as Duck, -radians);
                    this.Fondle(dart);
                    if (this.onFire)
                    {
                        Level.Add(SmallFire.New(0.0f, 0.0f, 0.0f, 0.0f, stick: dart, firedFrom: this));
                        dart.burning = true;
                        dart.onFire = true;
                    }
                    this._barrelHeat += 0.015f;
                    Vec2 vec = Maths.AngleToVec(radians);
                    dart.hSpeed = vec.x * 10f;
                    dart.vSpeed = vec.y * 10f;
                    Level.Add(dart);
                }
            }
            else
                this.DoAmmoClick();
        }

        public override void Fire()
        {
        }
    }
}
