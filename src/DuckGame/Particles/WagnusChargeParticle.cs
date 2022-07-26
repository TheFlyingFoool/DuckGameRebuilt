// Decompiled with JetBrains decompiler
// Type: DuckGame.WagnusChargeParticle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class WagnusChargeParticle : Thing, IFactory
    {
        private static int kMaxWagCharge = 64;
        private static WagnusChargeParticle[] _sparks = new WagnusChargeParticle[WagnusChargeParticle.kMaxWagCharge];
        private static int _lastActiveWagCharge = 0;
        private Thing _target;
        private float life = 1f;

        public static WagnusChargeParticle New(
          float xpos,
          float ypos,
          Thing target)
        {
            WagnusChargeParticle wagnusChargeParticle;
            if (WagnusChargeParticle._sparks[WagnusChargeParticle._lastActiveWagCharge] == null)
            {
                wagnusChargeParticle = new WagnusChargeParticle();
                WagnusChargeParticle._sparks[WagnusChargeParticle._lastActiveWagCharge] = wagnusChargeParticle;
            }
            else
                wagnusChargeParticle = WagnusChargeParticle._sparks[WagnusChargeParticle._lastActiveWagCharge];
            WagnusChargeParticle._lastActiveWagCharge = (WagnusChargeParticle._lastActiveWagCharge + 1) % WagnusChargeParticle.kMaxWagCharge;
            wagnusChargeParticle.ResetProperties();
            wagnusChargeParticle.Init(xpos, ypos, target);
            wagnusChargeParticle.globalIndex = Thing.GetGlobalIndex();
            return wagnusChargeParticle;
        }

        private WagnusChargeParticle()
          : base()
        {
        }

        private void Init(float xpos, float ypos, Thing target)
        {
            this.hSpeed = Rando.Float(-1f, 1f);
            this.vSpeed = Rando.Float(-1f, 1f);
            this.position.x = xpos;
            this.position.y = ypos;
            this.depth = (Depth)0.9f;
            this.life = 1f;
            this._target = target;
            this.alpha = 1f;
        }

        public override void Update()
        {
            Vec2 vec2 = this.position - this._target.position;
            float lengthSq = vec2.lengthSq;
            if ((double)lengthSq < 64.0 || (double)lengthSq > 4096.0)
                this.alpha -= 0.08f;
            this.hSpeed = Lerp.Float(this.hSpeed, (float)(-(double)vec2.x * 0.699999988079071), 0.15f);
            this.vSpeed = Lerp.Float(this.vSpeed, (float)(-(double)vec2.y * 0.699999988079071), 0.15f);
            this.position.x += this.hSpeed;
            this.position.y += this.vSpeed;
            this.position.x = Lerp.Float(this.position.x, this._target.x, 0.16f);
            this.position.y = Lerp.Float(this.position.y, this._target.y, 0.16f);
            this.hSpeed *= Math.Min(1f, (float)((double)lengthSq / 128.0 + 0.25));
            this.vSpeed *= Math.Min(1f, (float)((double)lengthSq / 128.0 + 0.25));
            this.life -= 0.02f;
            if ((double)this.life < 0.0)
                this.alpha -= 0.08f;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            base.Update();
        }

        public override void Draw() => Graphics.DrawLine(this.position, this.position + this.velocity.normalized * (this.velocity.length * 2f), new Color(147, 64, 221) * this.alpha, depth: this.depth);
    }
}
