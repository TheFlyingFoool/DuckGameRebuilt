// Decompiled with JetBrains decompiler
// Type: DuckGame.ConfettiParticle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class ConfettiParticle : PhysicsParticle, IFactory
    {
        private static int kMaxSparks = 64;
        private static ConfettiParticle[] _sparks = new ConfettiParticle[ConfettiParticle.kMaxSparks];
        private static int _lastActiveSpark = 0;
        private float _killSpeed = 0.03f;
        public Color _color;
        public float _width = 0.5f;
        private bool _stringConfetti;
        private static int _confettiNumber;
        private float life = 1f;
        private float sin;
        private float sinMult;

        public static ConfettiParticle New(
          float xpos,
          float ypos,
          Vec2 hitAngle,
          float killSpeed = 0.02f,
          bool lineType = false)
        {
            ConfettiParticle confettiParticle;
            if (ConfettiParticle._sparks[ConfettiParticle._lastActiveSpark] == null)
            {
                confettiParticle = new ConfettiParticle();
                ConfettiParticle._sparks[ConfettiParticle._lastActiveSpark] = confettiParticle;
            }
            else
                confettiParticle = ConfettiParticle._sparks[ConfettiParticle._lastActiveSpark];
            ConfettiParticle._lastActiveSpark = (ConfettiParticle._lastActiveSpark + 1) % ConfettiParticle.kMaxSparks;
            confettiParticle.ResetProperties();
            confettiParticle.Init(xpos, ypos, hitAngle, killSpeed);
            confettiParticle.globalIndex = Thing.GetGlobalIndex();
            confettiParticle._stringConfetti = lineType;
            return confettiParticle;
        }

        public ConfettiParticle()
          : base(0.0f, 0.0f)
        {
        }

        public void Init(float xpos, float ypos, Vec2 hitAngle, float killSpeed = 0.02f)
        {
            this.position.x = xpos;
            this.position.y = ypos;
            this.hSpeed = (-hitAngle.x * 1.5f) * Rando.Float(-2f, 2f);
            this.vSpeed = (-hitAngle.y * 2f * (Rando.Float(1f) - 0.3f)) - Rando.Float(1f);
            this.hSpeed *= 1.5f;
            this.vSpeed *= 1.5f;
            this._bounceEfficiency = 0.1f;
            this.depth = (Depth)0.9f;
            this._killSpeed = killSpeed;
            this._color = Color.RainbowColors[ConfettiParticle._confettiNumber % Color.RainbowColors.Count];
            ++ConfettiParticle._confettiNumber;
            this._width = 1f;
            this.life = Rando.Float(0.8f, 1f);
            this.sin = Rando.Float(3.14f);
            this._gravMult = 0.3f;
            this.sinMult = 0f;
            this.onlyDieWhenGrounded = true;
        }

        public override void Update()
        {
            this.hSpeed *= 0.95f;
            this.vSpeed *= 0.96f;
            this.life -= 0.03f;
            if (life <= 0f)
            {
                this.sinMult += 0.02f;
                if (sinMult > 1f)
                    this.sinMult = 1f;
                if (!this._grounded && (double)Math.Abs(this.hSpeed) < 0.2f)
                {
                    this.sin += 0.2f;
                    this.x += (float)(Math.Sin(sin) * 0.5) * this.sinMult;
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            if (this._stringConfetti)
            {
                Vec2 p2 = this.position + this.velocity.normalized * (this.velocity.length * (float)(3.0 + sinMult * 3.0));
                Vec2 position;
                Graphics.DrawLine(this.position, Level.CheckLine<Block>(this.position, p2, out position) != null ? position : p2, this._color * this.alpha, this._width, this.depth);
            }
            else
                Graphics.DrawRect(this.position + new Vec2(-1f, -1f), this.position + new Vec2(1f, 1f), this._color * this.alpha, this.depth);
        }
    }
}
