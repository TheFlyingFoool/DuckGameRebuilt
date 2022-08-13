// Decompiled with JetBrains decompiler
// Type: DuckGame.FireExtinguisher
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Fire")]
    [BaggedProperty("isFatal", false)]
    public class FireExtinguisher : Gun
    {
        public StateBinding _firingBinding = new StateBinding(nameof(_firing));
        private SpriteMap _guage;
        public bool _firing;
        private bool _smoke = true;
        private ConstantSound _sound;
        private int _maxAmmo = 200;

        public FireExtinguisher(float xval, float yval)
          : base(xval, yval)
        {
            ammo = _maxAmmo;
            _type = "gun";
            graphic = new Sprite("extinguisher");
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-3f, -8f);
            collisionSize = new Vec2(6f, 16f);
            _barrelOffsetTL = new Vec2(15f, 2f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 1f;
            _guage = new SpriteMap("netGunGuage", 8, 8);
            _holdOffset = new Vec2(0f, 2f);
            if (Network.isActive)
                ammo = 120;
            isFatal = false;
            editorTooltip = "Safety first! Extinguishes fires but also just makes everything fun and foamy.";
        }
        public override void Initialize()
        {
            _sound = new ConstantSound("flameThrowing");
            base.Initialize();
        }
        public override void Update()
        {
            base.Update();
            if (isServerForObject && _firing && ammo > 0)
            {
                if (_smoke)
                {
                    Vec2 vec = Maths.AngleToVec(barrelAngle + Rando.Float(-0.5f, 0.5f));
                    Vec2 vec2 = new Vec2(vec.x * Rando.Float(0.9f, 3f), vec.y * Rando.Float(0.9f, 3f));
                    ExtinguisherSmoke extinguisherSmoke = new ExtinguisherSmoke(barrelPosition.x, barrelPosition.y)
                    {
                        hSpeed = vec2.x,
                        vSpeed = vec2.y
                    };
                    --ammo;
                    _guage.frame = 3 - (int)(ammo / _maxAmmo * 4.0);
                    Level.Add(extinguisherSmoke);
                }
                _smoke = !_smoke;
            }
            else
                _smoke = true;
            _sound.lerpVolume = _firing ? 0.5f : 0f;
        }

        public override void Draw()
        {
            base.Draw();
            _guage.flipH = graphic.flipH;
            _guage.alpha = graphic.alpha;
            _guage.depth = depth + 1;
            Draw(_guage, new Vec2(-6f, -8f));
        }

        public override void OnPressAction() => _firing = true;

        public override void OnReleaseAction() => _firing = false;

        public override void Fire()
        {
        }
    }
}
