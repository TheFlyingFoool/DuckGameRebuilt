// Decompiled with JetBrains decompiler
// Type: DuckGame.JetpackSmoke
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public SpriteMap sprite => this._sprite;

        public JetpackSmoke(float xpos, float ypos)
          : base()
        {
            this._sprite = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num1 = Rando.Int(3) * 4;
            this._sprite.AddAnimation("idle", 0.1f, true, num1);
            this._sprite.AddAnimation("puff", Rando.Float(0.15f, 0.25f), false, num1, 1 + num1, 2 + num1, 3 + num1);
            this._orbiter = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num2 = Rando.Int(3) * 4;
            this._orbiter.AddAnimation("idle", 0.1f, true, num2);
            this._orbiter.AddAnimation("puff", Rando.Float(0.15f, 0.25f), false, num2, 1 + num2, 2 + num2, 3 + num2);
            this._sprite2 = new SpriteMap("tinySmokeTestBack", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.Init(xpos, ypos);
        }

        private void Init(float xpos, float ypos)
        {
            this._orbitInc += 0.2f;
            this._life = 1f;
            this.position.x = xpos;
            this.position.y = ypos;
            this._sprite.SetAnimation("idle");
            this._sprite.angleDegrees = Rando.Float(360f);
            this._orbiter.angleDegrees = Rando.Float(360f);
            this.s1 = Rando.Float(0.8f, 1.1f);
            this.s2 = Rando.Float(0.8f, 1.1f);
            this.hSpeed = Rando.Float(0.8f) - 0.4f;
            this.vSpeed = 0.1f + Rando.Float(0.4f);
            this._life += Rando.Float(0.2f);
            float num1 = 0.6f - Rando.Float(0.2f);
            float num2 = 1f;
            this._sprite.color = new Color(num2, num2, num2);
            this.depth = -0.4f;
            this.alpha = 1f;
            this.layer = Layer.Game;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            this.xscale = 1f;
            this.yscale = this.xscale;
            this._orbitInc += this._rotSpeed;
            this._distPulse += this._distPulseSpeed;
            this.vSpeed -= 0.01f;
            this.hSpeed *= 0.95f;
            this._life -= this.lifeTake;
            if (_life < 0.0 && this._sprite.currentAnimation != "puff")
                this._sprite.SetAnimation("puff");
            if (this._sprite.currentAnimation == "puff" && this._sprite.finished)
                Level.Remove(this);
            this.x += this.hSpeed;
            this.y += this.vSpeed;
        }

        public override void Draw()
        {
            float num1 = (float)Math.Sin(_distPulse);
            float num2 = (float)-(Math.Sin(_orbitInc) * (double)num1) * this.s1;
            float num3 = (float)Math.Cos(_orbitInc) * num1 * this.s1;
            this._sprite.imageIndex = this._sprite.imageIndex;
            this._sprite.depth = this.depth;
            this._sprite.scale = new Vec2(this.s1);
            this._sprite.center = this.center;
            Graphics.Draw(_sprite, this.x + num2, this.y + num3);
            this._sprite2.imageIndex = this._sprite.imageIndex;
            this._sprite2.angle = this._sprite.angle;
            this._sprite2.depth = -0.5f;
            this._sprite2.scale = this._sprite.scale;
            this._sprite2.center = this.center;
            float num4 = 0.6f - Rando.Float(0.2f);
            float num5 = 0.4f;
            this._sprite2.color = new Color(num5, num5, num5);
            Graphics.Draw(_sprite2, this.x + num2, this.y + num3);
            this._orbiter.imageIndex = this._sprite.imageIndex;
            this._orbiter.color = this._sprite.color;
            this._orbiter.depth = this.depth;
            this._orbiter.scale = new Vec2(this.s2);
            this._orbiter.center = this.center;
            Graphics.Draw(_orbiter, this.x - num2, this.y - num3);
            this._sprite2.imageIndex = this._orbiter.imageIndex;
            this._sprite2.angle = this._orbiter.angle;
            this._sprite2.depth = -0.5f;
            this._sprite2.scale = this._orbiter.scale;
            this._sprite2.center = this.center;
            this._sprite2.color = new Color(num5, num5, num5);
            Graphics.Draw(_sprite2, this.x - num2, this.y - num3);
        }
    }
}
