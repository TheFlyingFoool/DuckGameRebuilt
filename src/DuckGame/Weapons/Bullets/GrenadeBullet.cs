// Decompiled with JetBrains decompiler
// Type: DuckGame.GrenadeBullet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class GrenadeBullet : Bullet
    {
        private float _isVolatile = 1f;

        public GrenadeBullet(
          float xval,
          float yval,
          AmmoType type,
          float ang = -1f,
          Thing owner = null,
          bool rbound = false,
          float distance = -1f,
          bool tracer = false,
          bool network = true)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
        }

        protected override void OnHit(bool destroyed)
        {
            if (!destroyed || !this.isLocal)
                return;
            for (int index = 0; index < 1; ++index)
            {
                ExplosionPart explosionPart = new ExplosionPart(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                explosionPart.xscale *= 0.7f;
                explosionPart.yscale *= 0.7f;
                Level.Add(explosionPart);
            }
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(this.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
            foreach (MaterialThing materialThing in Level.CheckCircleAll<TV>(this.position, 20f))
                materialThing.Destroy(new DTImpact(this));
            List<Bullet> varBullets = new List<Bullet>();
            Vec2 vec2 = this.position - this.travelDirNormalized;
            for (int index = 0; index < 12; ++index)
            {
                float ang = (float)(index * 30.0 - 10.0) + Rando.Float(20f);
                ATGrenadeLauncherShrapnel type = new ATGrenadeLauncherShrapnel
                {
                    range = 25f + Rando.Float(10f)
                };
                Bullet bullet = new Bullet(vec2.x, vec2.y, type, ang)
                {
                    firedFrom = this
                };
                varBullets.Add(bullet);
                Level.Add(bullet);
            }
            if (Network.isActive && this.isLocal)
            {
                Send.Message(new NMFireGun(null, varBullets, 0, false), NetMessagePriority.ReliableOrdered);
                varBullets.Clear();
            }
            foreach (Window ignore in Level.CheckCircleAll<Window>(this.position, 20f))
            {
                if (Level.CheckLine<Block>(this.position, ignore.position, ignore) == null)
                    ignore.Destroy(new DTImpact(this));
            }
        }

        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            GrenadeBullet bullet = this.ammo.GetBullet(pos.x, pos.y, angle: (-dir), firedFrom: this.firedFrom, distance: rng, tracer: this._tracer) as GrenadeBullet;
            bullet._teleporter = this._teleporter;
            bullet._isVolatile = this._isVolatile;
            bullet.isLocal = this.isLocal;
            bullet.lastReboundSource = this.lastReboundSource;
            bullet.connection = this.connection;
            this.reboundCalled = true;
            Level.Add(bullet);
            SFX.Play("grenadeBounce", 0.8f, Rando.Float(-0.1f, 0.1f));
        }

        public override void Update()
        {
            this._isVolatile -= 0.06f;
            if (_isVolatile <= 0.0)
                this.rebound = false;
            base.Update();
        }
    }
}
