// Decompiled with JetBrains decompiler
// Type: DuckGame.SmallSmoke
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Diagnostics;

namespace DuckGame
{
    public class SmallSmoke : Thing
    {
        private static int kMaxObjects = 64;
        private static SmallSmoke[] _objects = new SmallSmoke[kMaxObjects];
        private static int _lastActiveObject = 0;
        public static bool shortlife = false;
        private float _orbitInc = Rando.Float(5f);
        private SpriteMap _sprite2;
        public SpriteMap _sprite;
        private SpriteMap _orbiter;
        private float _life = 1f;
        private float _rotSpeed = Rando.Float(0.05f, 0.15f);
        private float _distPulseSpeed = Rando.Float(0.05f, 0.15f);
        private float _distPulse = Rando.Float(5f);
        private float s1 = 1f;
        private float s2 = 1f;
        private float lifeTake = 0.05f;

        public static SmallSmoke New(float xpos, float ypos, float depth = 0.8f, float scaleMul = 1f)
        {
            SmallSmoke smallSmoke;
            if (NetworkDebugger.enabled)
                smallSmoke = new SmallSmoke();
            else if (_objects[_lastActiveObject] == null)
            {
                smallSmoke = new SmallSmoke();
                _objects[_lastActiveObject] = smallSmoke;
            }
            else
                smallSmoke = _objects[_lastActiveObject];
            _lastActiveObject = (_lastActiveObject + 1) % kMaxObjects;
            smallSmoke.Init(xpos, ypos);
            smallSmoke.ResetProperties();
            smallSmoke._sprite.globalIndex = GetGlobalIndex();
            smallSmoke.globalIndex = GetGlobalIndex();
            smallSmoke.depth = (Depth)depth;
            smallSmoke.s1 *= scaleMul;
            smallSmoke.s2 *= scaleMul;
            if (shortlife)
                smallSmoke.lifeTake = 0.14f;
            return smallSmoke;
        }

        public static SmallSmoke New(float xpos, float ypos)
        {
            SmallSmoke smallSmoke;
            if (_objects[_lastActiveObject] == null)
            {
                smallSmoke = new SmallSmoke();
                _objects[_lastActiveObject] = smallSmoke;
            }
            else
                smallSmoke = _objects[_lastActiveObject];
            _lastActiveObject = (_lastActiveObject + 1) % kMaxObjects;
            smallSmoke.Init(xpos, ypos);
            smallSmoke.ResetProperties();
            smallSmoke._sprite.globalIndex = GetGlobalIndex();
            smallSmoke.globalIndex = GetGlobalIndex();
            smallSmoke.depth = (Depth)0.8f;
            return smallSmoke;
        }

        public SpriteMap sprite => _sprite;

        private SmallSmoke()
          : base()
        {
            _sprite = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num1 = Rando.Int(3) * 4;
            _sprite.AddAnimation("idle", 0.1f, true, num1);
            _sprite.AddAnimation("puff", Rando.Float(0.15f, 0.25f), false, num1, 1 + num1, 2 + num1, 3 + num1);
            _orbiter = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num2 = Rando.Int(3) * 4;
            _orbiter.AddAnimation("idle", 0.1f, true, num2);
            _orbiter.AddAnimation("puff", Rando.Float(0.15f, 0.25f), false, num2, 1 + num2, 2 + num2, 3 + num2);
            _sprite2 = new SpriteMap("tinySmokeTestBack", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
        }

        private void Init(float xpos, float ypos)
        {
            _orbitInc += 0.2f;
            _life = 1f;
            position.x = xpos;
            position.y = ypos;
            _sprite.SetAnimation("idle");
            _sprite.angle = Rando.Float(6.28319f);
            _orbiter.angle = Rando.Float(6.28319f);
            s1 = Rando.Float(0.6f, 1f);
            s2 = Rando.Float(0.6f, 1f);
            hSpeed = Rando.Float(-0.15f, 0.15f);
            vSpeed = Rando.Float(-0.15f, 0.1f);
            _life += Rando.Float(0.2f);
            float num1 = 0.6f - Rando.Float(0.2f);
            float num2 = 0.7f;
            _sprite.color = new Color(num2, num2, num2);
            depth = (Depth)0.8f;
            alpha = 1f;
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
            vSpeed -= 0.01f;
            hSpeed *= 0.95f;
            _life -= lifeTake;

            //since the lifetime of this particle is tied to its animation when its being culled the animation doesn't progress
            //so instead its just getting called here so it can delete properly -NiK0
            if (currentlyDrawing) _sprite.UpdateFrame();

            if (_life < 0 && _sprite.currentAnimation != "puff")
                _sprite.SetAnimation("puff");
            if (_sprite.currentAnimation == "puff" && _sprite.finished)
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
            Graphics.Draw(_sprite, x + num2, y + num3);
            _sprite2.imageIndex = _sprite.imageIndex;
            _sprite2.angle = _sprite.angle;
            _sprite2.depth = -0.5f;
            _sprite2.scale = _sprite.scale;
            _sprite2.center = center;
            float num4 = 0.6f - Rando.Float(0.2f);
            float num5 = 0.4f;
            _sprite2.color = new Color(num5, num5, num5);
            Graphics.Draw(_sprite2, x + num2, y + num3);
            _orbiter.imageIndex = _sprite.imageIndex;
            _orbiter.color = _sprite.color;
            _orbiter.depth = depth;
            _orbiter.scale = new Vec2(s2);
            _orbiter.center = center;
            Graphics.Draw(_orbiter, x - num2, y - num3);
            _sprite2.imageIndex = _orbiter.imageIndex;
            _sprite2.angle = _orbiter.angle;
            _sprite2.depth = -0.5f;
            _sprite2.scale = _orbiter.scale;
            _sprite2.center = center;
            _sprite2.color = new Color(num5, num5, num5);
            Graphics.Draw(_sprite2, x - num2, y - num3);
        }
    }
}
