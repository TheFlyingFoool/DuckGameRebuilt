// Decompiled with JetBrains decompiler
// Type: DuckGame.BreathSmoke
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class BreathSmoke : Thing
    {
        private static int kMaxObjects = 64;
        private static BreathSmoke[] _objects = new BreathSmoke[BreathSmoke.kMaxObjects];
        private static int _lastActiveObject = 0;
        public static bool shortlife = false;
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

        public static BreathSmoke New(float xpos, float ypos, float depth = 0.8f, float scaleMul = 1f)
        {
            BreathSmoke breathSmoke;
            if (BreathSmoke._objects[BreathSmoke._lastActiveObject] == null)
            {
                breathSmoke = new BreathSmoke();
                BreathSmoke._objects[BreathSmoke._lastActiveObject] = breathSmoke;
            }
            else
                breathSmoke = BreathSmoke._objects[BreathSmoke._lastActiveObject];
            BreathSmoke._lastActiveObject = (BreathSmoke._lastActiveObject + 1) % BreathSmoke.kMaxObjects;
            breathSmoke.Init(xpos, ypos);
            breathSmoke.ResetProperties();
            breathSmoke._sprite.globalIndex = (int)Thing.GetGlobalIndex();
            breathSmoke.globalIndex = Thing.GetGlobalIndex();
            breathSmoke.depth = (Depth)depth;
            breathSmoke.s1 *= scaleMul;
            breathSmoke.s2 *= scaleMul;
            if (BreathSmoke.shortlife)
                breathSmoke.lifeTake = 0.14f;
            return breathSmoke;
        }

        public static BreathSmoke New(float xpos, float ypos)
        {
            BreathSmoke breathSmoke;
            if (BreathSmoke._objects[BreathSmoke._lastActiveObject] == null)
            {
                breathSmoke = new BreathSmoke();
                BreathSmoke._objects[BreathSmoke._lastActiveObject] = breathSmoke;
            }
            else
                breathSmoke = BreathSmoke._objects[BreathSmoke._lastActiveObject];
            BreathSmoke._lastActiveObject = (BreathSmoke._lastActiveObject + 1) % BreathSmoke.kMaxObjects;
            breathSmoke.Init(xpos, ypos);
            breathSmoke.ResetProperties();
            breathSmoke._sprite.globalIndex = (int)Thing.GetGlobalIndex();
            breathSmoke.globalIndex = Thing.GetGlobalIndex();
            breathSmoke.depth = (Depth)0.8f;
            return breathSmoke;
        }

        public SpriteMap sprite => this._sprite;

        private BreathSmoke()
          : base()
        {
            this._sprite = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num1 = Rando.Int(3) * 4;
            this._sprite.AddAnimation("idle", 0.1f, true, 2 + num1);
            this._sprite.AddAnimation("puff", Rando.Float(0.08f, 0.12f), false, 2 + num1, 1 + num1, num1);
            this._orbiter = new SpriteMap("tinySmokeTestFront", 16, 16);
            int num2 = Rando.Int(3) * 4;
            this._orbiter.AddAnimation("idle", 0.1f, true, 2 + num2);
            this._orbiter.AddAnimation("puff", Rando.Float(0.08f, 0.12f), false, 2 + num2, 1 + num2, num2);
            this._sprite2 = new SpriteMap("tinySmokeTestBack", 16, 16);
            this._sprite2.currentAnimation = (string)null;
            this._orbiter.currentAnimation = (string)null;
            this.center = new Vec2(8f, 8f);
        }

        private void Init(float xpos, float ypos)
        {
            this._orbitInc += 0.2f;
            this._life = 1f;
            this.position.x = xpos;
            this.position.y = ypos;
            this._sprite.SetAnimation("idle");
            this._sprite.frame = 0;
            this._orbiter.imageIndex = this._sprite.imageIndex;
            this._sprite2.imageIndex = this._sprite.imageIndex;
            this._sprite.angleDegrees = Rando.Float(360f);
            this._orbiter.angleDegrees = Rando.Float(360f);
            this.s1 = Rando.Float(0.6f, 1f);
            this.s2 = Rando.Float(0.6f, 1f);
            this.hSpeed = Rando.Float(-0.15f, 0.15f);
            this.vSpeed = Rando.Float(-0.1f, -0.05f);
            this._life += Rando.Float(0.2f);
            this._sprite.color = Color.White;
            this.depth = (Depth)0.8f;
            this.alpha = 0.15f;
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
            this.alpha -= 3f / 1000f;
            this.vSpeed -= 0.01f;
            this.hSpeed *= 0.95f;
            if (this._sprite.currentAnimation != "puff")
                this._sprite.SetAnimation("puff");
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            this.x += this.hSpeed;
            this.y += this.vSpeed;
        }

        public override void Draw()
        {
            float num1 = (float)Math.Sin((double)this._distPulse);
            float num2 = (float)-(Math.Sin((double)this._orbitInc) * (double)num1) * this.s1;
            float num3 = (float)Math.Cos((double)this._orbitInc) * num1 * this.s1;
            this._sprite.imageIndex = this._sprite.imageIndex;
            this._sprite.depth = this.depth;
            this._sprite.scale = new Vec2(this.s1);
            this._sprite.center = this.center;
            this._sprite.alpha = this.alpha;
            this._sprite.color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)((double)this.alpha * (double)byte.MaxValue));
            this._sprite.color = Color.White * this.alpha;
            Graphics.Draw((Sprite)this._sprite, this.x + num2, this.y + num3);
            this._sprite2.frame = 0;
            this._sprite2.imageIndex = this._sprite.imageIndex;
            this._sprite2.angle = this._sprite.angle;
            this._sprite2.depth = - 0.5f;
            this._sprite2.scale = this._sprite.scale;
            this._sprite2.center = this.center;
            double num4 = (double)Rando.Float(0.2f);
            this._sprite2.color = this._sprite.color;
            Graphics.Draw((Sprite)this._sprite2, this.x + num2, this.y + num3);
        }
    }
}
