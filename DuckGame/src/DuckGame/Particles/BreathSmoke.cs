using System;

namespace DuckGame
{
    public class BreathSmoke : Thing
    {
        public static int kMaxObjects = 64;
        public static BreathSmoke[] _objects = new BreathSmoke[kMaxObjects];
        public static int _lastActiveObject = 0;
        public static bool shortlife = false;
        private float _orbitInc = Rando.Float(5f);
        private SpriteMap _sprite2;
        private SpriteMap _sprite;
        private SpriteMap _orbiter;
        //private float _life = 1f;
        private float _rotSpeed = Rando.Float(0.05f, 0.15f);
        private float _distPulseSpeed = Rando.Float(0.05f, 0.15f);
        private float _distPulse = Rando.Float(5f);
        private float s1 = 1f;
        private float s2 = 1f;
        private static int colorindex;

        //private float lifeTake = 0.05f;

        public static BreathSmoke New(float xpos, float ypos, float depth = 0.8f, float scaleMul = 1f)
        {
            BreathSmoke breathSmoke;
            if (_objects[_lastActiveObject] == null)
            {
                breathSmoke = new BreathSmoke();
                _objects[_lastActiveObject] = breathSmoke;
            }
            else
                breathSmoke = _objects[_lastActiveObject];
            _lastActiveObject = (_lastActiveObject + 1) % kMaxObjects;
            breathSmoke.Init(xpos, ypos);
            breathSmoke.ResetProperties();
            breathSmoke._sprite.globalIndex = GetGlobalIndex();
            breathSmoke.globalIndex = GetGlobalIndex();
            breathSmoke.depth = (Depth)depth;
            breathSmoke.s1 *= scaleMul;
            breathSmoke.s2 *= scaleMul;
            //if (BreathSmoke.shortlife)
            //breathSmoke.lifeTake = 0.14f;
            return breathSmoke;
        }

        public static BreathSmoke New(float xpos, float ypos)
        {
            BreathSmoke breathSmoke;
            if (_objects[_lastActiveObject] == null)
            {
                breathSmoke = new BreathSmoke();
                _objects[_lastActiveObject] = breathSmoke;
            }
            else
                breathSmoke = _objects[_lastActiveObject];
            _lastActiveObject = (_lastActiveObject + 1) % kMaxObjects;
            breathSmoke.Init(xpos, ypos);
            breathSmoke.ResetProperties();
            breathSmoke._sprite.globalIndex = GetGlobalIndex();
            breathSmoke.globalIndex = GetGlobalIndex();
            breathSmoke.depth = (Depth)0.8f;
            return breathSmoke;
        }

        public SpriteMap sprite => _sprite;

        private BreathSmoke()
          : base()
        {
            _sprite = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num1 = Rando.Int(3) * 4;
            _sprite.AddAnimation("idle", 0.1f, true, 2 + num1);
            _sprite.AddAnimation("puff", Rando.Float(0.08f, 0.12f), false, 2 + num1, 1 + num1, num1);
            _orbiter = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num2 = Rando.Int(3) * 4;
            _orbiter.AddAnimation("idle", 0.1f, true, 2 + num2);
            _orbiter.AddAnimation("puff", Rando.Float(0.08f, 0.12f), false, 2 + num2, 1 + num2, num2);
            _sprite2 = new SpriteMap("tinySmokeTestBack", 16, 16)
            {
                currentAnimation = null
            };
            _orbiter.currentAnimation = null;
            center = new Vec2(8f, 8f);
        }

        private void Init(float xpos, float ypos)
        {
            _orbitInc += 0.2f;
            //this._life = 1f;
            position.x = xpos;
            position.y = ypos;
            _sprite.SetAnimation("idle");
            _sprite.frame = 0;
            _orbiter.imageIndex = _sprite.imageIndex;
            _sprite2.imageIndex = _sprite.imageIndex;
            _sprite.angle = Rando.Float(6.28319f);
            _orbiter.angle = Rando.Float(6.28319f);
            s1 = Rando.Float(0.6f, 1f);
            s2 = Rando.Float(0.6f, 1f);
            hSpeed = Rando.Float(-0.15f, 0.15f);
            vSpeed = Rando.Float(-0.1f, -0.05f);
            //this._life += Rando.Float(0.2f);
            _sprite.color = Color.White;
            depth = (Depth)0.8f;
            alpha = 0.15f;
            layer = Layer.Game;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            xscale = 1f;
            yscale = xscale;
            _orbitInc += _rotSpeed;
            _distPulse += _distPulseSpeed;
            alpha -= 3f / 1000f;
            vSpeed -= 0.01f;
            hSpeed *= 0.95f;
            if (_sprite.currentAnimation != "puff")
                _sprite.SetAnimation("puff");
            if (alpha < 0f)
                Level.Remove(this);
            x += hSpeed;
            y += vSpeed;
        }

        public override void Draw()
        {
            float num1 = (float)Math.Sin(_distPulse);
            float num2 = (float)-(Math.Sin(_orbitInc) * num1) * s1;
            float num3 = (float)Math.Cos(_orbitInc) * num1 * s1;
            _sprite.imageIndex = _sprite.imageIndex;
            _sprite.depth = depth;
            _sprite.scale = new Vec2(s1);
            _sprite.center = center;
            _sprite.alpha = alpha;
            _sprite.color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(alpha * byte.MaxValue));
            _sprite.color = Color.White * alpha;
            Graphics.Draw(ref _sprite, x + num2, y + num3);
            _sprite2.frame = 0;
            _sprite2.imageIndex = _sprite.imageIndex;
            _sprite2.angle = _sprite.angle;
            _sprite2.depth = -0.5f;
            _sprite2.scale = _sprite.scale;
            _sprite2.center = center;
            double num4 = Rando.Float(0.2f);
            _sprite2.color = _sprite.color;
            Graphics.Draw(ref _sprite2, x + num2, y + num3);
        }
    }
}
