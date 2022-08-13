// Decompiled with JetBrains decompiler
// Type: DuckGame.WagnusChargeParticle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            hSpeed = Rando.Float(-1f, 1f);
            vSpeed = Rando.Float(-1f, 1f);
            position.x = xpos;
            position.y = ypos;
            depth = (Depth)0.9f;
            life = 1f;
            _target = target;
            alpha = 1f;
        }

        public override void Update()
        {
            Vec2 vec2 = position - _target.position;
            float lengthSq = vec2.lengthSq;
            if (lengthSq < 64.0 || lengthSq > 4096.0)
                alpha -= 0.08f;
            hSpeed = Lerp.Float(hSpeed, (float)(-vec2.x *  0.7f), 0.15f);
            vSpeed = Lerp.Float(vSpeed, (float)(-vec2.y *  0.7f), 0.15f);
            position.x += hSpeed;
            position.y += vSpeed;
            position.x = Lerp.Float(position.x, _target.x, 0.16f);
            position.y = Lerp.Float(position.y, _target.y, 0.16f);
            hSpeed *= Math.Min(1f, (float)(lengthSq / 128.0 + 0.25));
            vSpeed *= Math.Min(1f, (float)(lengthSq / 128.0 + 0.25));
            life -= 0.02f;
            if (life < 0.0)
                alpha -= 0.08f;
            if (alpha < 0.0)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw() => Graphics.DrawLine(position, position + velocity.normalized * (velocity.length * 2f), new Color(147, 64, 221) * alpha, depth: depth);
    }
}
