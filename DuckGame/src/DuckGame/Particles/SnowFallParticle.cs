// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowFallParticle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class SnowFallParticle : PhysicsParticle
    {
        private float _sin;
        private float _moveSpeed = 0.1f;
        private float _sinSize = 0.1f;
        //private float _drift;
        public float _size;
        public bool slowDis;
        public SnowFallParticle(float xpos, float ypos, Vec2 startVel, bool big = false)
          : base(xpos, ypos)
        {
            _gravMult = 0.5f;
            _sin = Rando.Float(7f);
            _moveSpeed = Rando.Float(0.005f, 0.02f);
            _sinSize = Rando.Float(8f, 16f);
            _size = Rando.Float(0.2f, 0.6f);
            if (big)
                _size = Rando.Float(0.8f, 1f);
            life = Rando.Float(0.1f, 0.2f);
            onlyDieWhenGrounded = true;
            velocity = startVel;
        }

        public override void Update()
        {
            base.Update();
            if (vSpeed > 1)
                vSpeed = 1f;
            if (_grounded)
            {
                if (slowDis)
                {
                    alpha -= 0.001f;
                    if (alpha <= 0)
                    {
                        Level.Remove(this);
                    }
                }
                return;
            }
            float num = (float)Math.Sin(_sin) * _sinSize;
            _sin += _moveSpeed;
            x += Rando.Float(-0.3f, 0.3f);
            x += num / 60f;
        }

        public override void Draw()
        {
            float size = _size;
            Graphics.DrawRect(position + new Vec2(-size, -size), position + new Vec2(size, size), Color.White * alpha, (Depth)0.1f);
        }
    }
}
