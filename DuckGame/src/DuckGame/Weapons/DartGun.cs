// Decompiled with JetBrains decompiler
// Type: DuckGame.DartGun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            ammo = 12;
            _ammoType = new ATLaser
            {
                range = 170f,
                accuracy = 0.8f
            };
            wideBarrel = true;
            barrelInsertOffset = new Vec2(-3f, -1f);
            _type = "gun";
            _sprite = new SpriteMap("dartgun", 32, 32);
            graphic = _sprite;
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -4f);
            collisionSize = new Vec2(16f, 9f);
            _barrelOffsetTL = new Vec2(29f, 14f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            flammable = 0.8f;
            _barrelAngleOffset = 8f;
            physicsMaterial = PhysicsMaterial.Plastic;
            editorTooltip = "Shoots like a gun, feels like a gentle breeze.";
            isFatal = false;
        }

        public override void Initialize() => base.Initialize();

        public override void UpdateFirePosition(SmallFire f) => f.position = Offset(new Vec2(10f, 0f));

        public override void UpdateOnFire()
        {
            if (!onFire)
                return;
            _burnWait -= 0.01f;
            if (_burnWait < 0)
            {
                Level.Add(SmallFire.New(10f, 0f, 0f, 0f, stick: this, canMultiply: false, firedFrom: this));
                _burnWait = 1f;
            }
            if (burnt >= 1f)
                return;
            burnt += 1f / 1000f;
        }

        public override void Update()
        {
            if (!burntOut && burnt >= 1f)
            {
                _sprite.frame = 1;
                Vec2 vec2 = Offset(new Vec2(10f, 0f));
                if (DGRSettings.ActualParticleMultiplier >= 1) for (int i = 0; i < DGRSettings.ActualParticleMultiplier; i++) Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                else if (Rando.Float(1) < DGRSettings.ActualParticleMultiplier) Level.Add(SmallSmoke.New(vec2.x, vec2.y));
                _onFire = false;
                flammable = 0f;
                burntOut = true;
            }
            base.Update();
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            onFire = true;
            return true;
        }

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                if (_burnLife <= 0)
                {
                    SFX.Play("dartStick", 0.5f, Rando.Float(0.2f) - 0.1f);
                }
                else
                {
                    --ammo;
                    SFX.Play("dartGunFire", 0.5f, Rando.Float(0.2f) - 0.1f);
                    kick = 1f;
                    if (receivingPress || !isServerForObject)
                        return;
                    Vec2 vec2 = Offset(barrelOffset + new Vec2(-8f, 0f));
                    float radians = barrelAngle + Rando.Float(-0.05f, 0.05f);
                    Dart dart = new Dart(vec2.x, vec2.y, owner as Duck, -radians);
                    Fondle(dart);
                    if (onFire)
                    {
                        Level.Add(SmallFire.New(0f, 0f, 0f, 0f, stick: dart, firedFrom: this));
                        dart.burning = true;
                        dart.onFire = true;
                    }
                    _barrelHeat += 0.015f;
                    Vec2 vec = Maths.AngleToVec(radians);
                    dart.hSpeed = vec.x * 10f;
                    dart.vSpeed = vec.y * 10f;
                    Level.Add(dart);
                }
            }
            else
                DoAmmoClick();
        }

        public override void Fire()
        {
        }
    }
}
