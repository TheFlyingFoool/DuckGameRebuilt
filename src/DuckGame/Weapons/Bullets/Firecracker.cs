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

        public Firecracker(float xpos, float ypos, float ang = 0f)
          : base(xpos, ypos)
        {
            graphic = new Sprite("fireCracker");
            center = new Vec2(4f, 4f);
            _bounceSound = "plasticBounce";
            _airFriction = 0.02f;
            _bounceEfficiency = 0.65f;
            _spinAngle = ang;
            isLocal = true;
            if (!Network.isActive)
                return;
            GhostManager.context.particleManager.AddLocalParticle(this);
        }

        public Firecracker(float xpos, float ypos, bool network)
          : this(xpos, ypos)
        {
            if (Network.isActive && !network)
                GhostManager.context.particleManager.AddLocalParticle(this);
            isLocal = !network;
        }

        public override void NetSerialize(BitBuffer b)
        {
            b.Write((short)x);
            b.Write((short)y);
            b.Write(_spinAngle);
        }

        public override void NetDeserialize(BitBuffer d)
        {
            netLerpPosition = new Vec2(d.ReadShort(), d.ReadShort());
            _spinAngle = d.ReadFloat();
        }

        public override void Removed()
        {
            if (Network.isActive && !didRemove)
            {
                didRemove = true;
                if (isLocal && GhostManager.context != null)
                {
                    GhostManager.context.particleManager.RemoveParticle(this);
                }
                else
                {
                    position = netLerpPosition;
                    Level.Add(SmallSmoke.New(x, y));
                }
            }
            base.Removed();
        }

        public override void Update()
        {
            if ((bool)_sparkTimer)
                Level.Add(Spark.New(x, y - 2f, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.1f));
            _life = 1f;
            angleDegrees = _spinAngle;
            base.Update();
            if (!isLocal || !(bool)_explodeTimer)
                return;
            SFX.PlaySynchronized("littleGun", Rando.Float(0.8f, 1f), Rando.Float(-0.5f, 0.5f));
            List<Bullet> varBullets = new List<Bullet>();
            for (int index = 0; index < 8; ++index)
            {
                float num = (float)(index * 45.0 - 5.0) + Rando.Float(10f);
                ATShrapnel type = new ATShrapnel
                {
                    range = 8f + Rando.Float(3f)
                };
                Bullet bullet = new Bullet(x + (float)(Math.Cos(Maths.DegToRad(num)) * 6.0), y - (float)(Math.Sin(Maths.DegToRad(num)) * 6.0), type, num)
                {
                    firedFrom = this
                };
                Level.Add(bullet);
                varBullets.Add(bullet);
            }
            if (Network.isActive)
                Send.Message(new NMFireGun(null, varBullets, 0, false), NetMessagePriority.ReliableOrdered);
            Level.Add(SmallSmoke.New(x, y));
            if (Rando.Float(1f) < 0.1f)
                Level.Add(SmallFire.New(x, y, 0f, 0f, firedFrom: this));
            Level.Remove(this);
        }
    }
}
