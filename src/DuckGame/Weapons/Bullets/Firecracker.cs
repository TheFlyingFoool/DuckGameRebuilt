// Decompiled with JetBrains decompiler
// Type: DuckGame.Firecracker
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class Firecracker : PhysicsParticle, ITeleport
    {
        private ActionTimer _sparkTimer = (ActionTimer)0.2f;
        private ActionTimer _explodeTimer = (ActionTimer)Rando.Float(0.01f, 0.012f);
        private bool didRemove;

        public Firecracker(float xpos, float ypos, float ang = 0.0f)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("fireCracker");
            this.center = new Vec2(4f, 4f);
            this._bounceSound = "plasticBounce";
            this._airFriction = 0.02f;
            this._bounceEfficiency = 0.65f;
            this._spinAngle = ang;
            this.isLocal = true;
            if (!Network.isActive)
                return;
            GhostManager.context.particleManager.AddLocalParticle((PhysicsParticle)this);
        }

        public Firecracker(float xpos, float ypos, bool network)
          : this(xpos, ypos)
        {
            if (Network.isActive && !network)
                GhostManager.context.particleManager.AddLocalParticle((PhysicsParticle)this);
            this.isLocal = !network;
        }

        public override void NetSerialize(BitBuffer b)
        {
            b.Write((short)this.x);
            b.Write((short)this.y);
            b.Write(this._spinAngle);
        }

        public override void NetDeserialize(BitBuffer d)
        {
            this.netLerpPosition = new Vec2((float)d.ReadShort(), (float)d.ReadShort());
            this._spinAngle = d.ReadFloat();
        }

        public override void Removed()
        {
            if (Network.isActive && !this.didRemove)
            {
                this.didRemove = true;
                if (this.isLocal && GhostManager.context != null)
                {
                    GhostManager.context.particleManager.RemoveParticle((PhysicsParticle)this);
                }
                else
                {
                    this.position = this.netLerpPosition;
                    Level.Add((Thing)SmallSmoke.New(this.x, this.y));
                }
            }
            base.Removed();
        }

        public override void Update()
        {
            if ((bool)this._sparkTimer)
                Level.Add((Thing)Spark.New(this.x, this.y - 2f, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.1f));
            this._life = 1f;
            this.angleDegrees = this._spinAngle;
            base.Update();
            if (!this.isLocal || !(bool)this._explodeTimer)
                return;
            SFX.PlaySynchronized("littleGun", Rando.Float(0.8f, 1f), Rando.Float(-0.5f, 0.5f));
            List<Bullet> varBullets = new List<Bullet>();
            for (int index = 0; index < 8; ++index)
            {
                float num = (float)((double)index * 45.0 - 5.0) + Rando.Float(10f);
                ATShrapnel type = new ATShrapnel();
                type.range = 8f + Rando.Float(3f);
                Bullet bullet = new Bullet(this.x + (float)(Math.Cos((double)Maths.DegToRad(num)) * 6.0), this.y - (float)(Math.Sin((double)Maths.DegToRad(num)) * 6.0), (AmmoType)type, num);
                bullet.firedFrom = (Thing)this;
                Level.Add((Thing)bullet);
                varBullets.Add(bullet);
            }
            if (Network.isActive)
                Send.Message((NetMessage)new NMFireGun((Gun)null, varBullets, (byte)0, false), NetMessagePriority.ReliableOrdered);
            Level.Add((Thing)SmallSmoke.New(this.x, this.y));
            if ((double)Rando.Float(1f) < 0.100000001490116)
                Level.Add((Thing)SmallFire.New(this.x, this.y, 0.0f, 0.0f, firedFrom: ((Thing)this)));
            Level.Remove((Thing)this);
        }
    }
}
