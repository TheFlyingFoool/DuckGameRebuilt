// Decompiled with JetBrains decompiler
// Type: DuckGame.ConfettiParticle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class ConfettiParticle : PhysicsParticle, IFactory
    {
        public static int kMaxSparks = 64;
        public static ConfettiParticle[] _sparks = new ConfettiParticle[ConfettiParticle.kMaxSparks];
        public static int _lastActiveSpark = 0;
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
          : base(0f, 0f)
        {
        }

        public void Init(float xpos, float ypos, Vec2 hitAngle, float killSpeed = 0.02f)
        {
            position.x = xpos;
            position.y = ypos;
            hSpeed = (-hitAngle.x * 1.5f) * Rando.Float(-2f, 2f);
            vSpeed = (-hitAngle.y * 2f * (Rando.Float(1f) - 0.3f)) - Rando.Float(1f);
            hSpeed *= 1.5f;
            vSpeed *= 1.5f;
            _bounceEfficiency = 0.1f;
            depth = (Depth)0.9f;
            _killSpeed = killSpeed;
            _color = Color.RainbowColors[ConfettiParticle._confettiNumber % Color.RainbowColors.Count];
            ++ConfettiParticle._confettiNumber;
            _width = 1f;
            life = Rando.Float(0.8f, 1f);
            sin = Rando.Float(3.14f);
            _gravMult = 0.3f;
            sinMult = 0f;
            onlyDieWhenGrounded = true;
        }

        public override void Update()
        {
            hSpeed *= 0.95f;
            vSpeed *= 0.96f;
            life -= 0.03f;
            if (life <= 0f)
            {
                sinMult += 0.02f;
                if (sinMult > 1f)
                    sinMult = 1f;
                if (!_grounded && Math.Abs(hSpeed) < 0.2f)
                {
                    sin += 0.2f;
                    x += (float)(Math.Sin(sin) * 0.5f) * sinMult;
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            if (_stringConfetti)
            {
                Vec2 p2 = this.position + velocity.normalized * (velocity.length * (float)(3.0 + sinMult * 3.0));
                Vec2 position;
                Graphics.DrawLine(this.position, Level.CheckLine<Block>(this.position, p2, out position) != null ? position : p2, _color * alpha, _width, depth);
            }
            else
                Graphics.DrawRect(position + new Vec2(-1f, -1f), position + new Vec2(1f, 1f), _color * alpha, depth);
        }
    }
}
