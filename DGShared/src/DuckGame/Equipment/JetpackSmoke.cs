// Decompiled with JetBrains decompiler
// Type: DuckGame.JetpackSmoke
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class JetpackSmoke : Thing
    {
        private float _orbitInc = Rando.Float(5f);
        private SpriteMap _sprite2;
        private SpriteMap _sprite;
        private SpriteMap _orbiter;
        private float _life = 1f;
        private float _rotSpeed = Rando.Float(0.05f, 0.15f);
        private float _distPulseSpeed = Rando.Float(0.05f, 0.15f);
        private float _distPulse = Rando.Float(5f);
        private float s1 = 1f;
        private float s2 = 1f;
        private float lifeTake = 0.05f;
        private static int colorindex;

        public SpriteMap sprite => _sprite;

        public JetpackSmoke(float xpos, float ypos)
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
            Init(xpos, ypos);
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
            s1 = Rando.Float(0.8f, 1.1f);
            s2 = Rando.Float(0.8f, 1.1f);
            hSpeed = Rando.Float(0.8f) - 0.4f;
            vSpeed = 0.1f + Rando.Float(0.4f);
            _life += Rando.Float(0.2f);
            //float num1 = 0.6f - Rando.Float(0.2f);
            float lightness = 1f;
            _sprite.color = new Color(lightness, lightness, lightness);
            if (Program.gay) //Program.gay
            {
                _sprite.color = Colors.Rainbow[colorindex];
                colorindex += 1;
                if (colorindex >= Colors.Rainbow.Length)
                {
                    colorindex = 0;
                }
            }
            depth = -0.4f;
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
            if (_life < 0.0 && _sprite.currentAnimation != "puff")
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
