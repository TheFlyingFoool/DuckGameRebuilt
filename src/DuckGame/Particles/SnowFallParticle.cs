// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowFallParticle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
        private float _size;

        public SnowFallParticle(float xpos, float ypos, Vec2 startVel, bool big = false)
          : base(xpos, ypos)
        {
            this._gravMult = 0.5f;
            this._sin = Rando.Float(7f);
            this._moveSpeed = Rando.Float(0.005f, 0.02f);
            this._sinSize = Rando.Float(8f, 16f);
            this._size = Rando.Float(0.2f, 0.6f);
            if (big)
                this._size = Rando.Float(0.8f, 1f);
            this.life = Rando.Float(0.1f, 0.2f);
            this.onlyDieWhenGrounded = true;
            this.velocity = startVel;
        }

        public override void Update()
        {
            base.Update();
            if ((double)this.vSpeed > 1.0)
                this.vSpeed = 1f;
            if (this._grounded)
                return;
            float num = (float)Math.Sin((double)this._sin) * this._sinSize;
            this._sin += this._moveSpeed;
            this.x += Rando.Float(-0.3f, 0.3f);
            this.x += num / 60f;
        }

        public override void Draw()
        {
            double num = (double)this.z / 200.0;
            float size = this._size;
            Graphics.DrawRect(this.position + new Vec2(-size, -size), this.position + new Vec2(size, size), Color.White * this.alpha, (Depth)0.1f);
        }
    }
}
