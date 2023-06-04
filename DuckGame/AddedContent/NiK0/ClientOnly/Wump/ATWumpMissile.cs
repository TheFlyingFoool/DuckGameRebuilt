// Decompiled with JetBrains decompiler
// Type: DuckGame.ATMissile
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class ATWumpMissile : AmmoType
    {
        public ATWumpMissile()
        {
            accuracy = 1f;
            range = 850f;
            penetration = 0.4f;
            bulletSpeed = 4f;
            bulletThickness = 6f;
            sprite = new Sprite("wmissile");
            bulletType = typeof(WumpMissile);
            sprite.CenterOrigin();
            speedVariation = 0f;
            flawlessPipeTravel = true;
            affectedByGravity = true;
        }
        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f))
            };
            Level.Add(pistolShell);
        }
        public static int lol = 1;
        public override Bullet FireBullet(Vec2 position, Thing owner = null, float angle = 0, Thing firedFrom = null)
        {
            lol *= -1;
            return base.FireBullet(position, owner, angle, firedFrom);
        }
        public override void OnHit(bool destroyed, Bullet b)
        {
            if (!b.isLocal)
                return;
            if (destroyed)
            {
                RumbleManager.AddRumbleEvent(b.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
                new ATMissileShrapnel().MakeNetEffect(b.position, false);
                Random random = null;
                if (Network.isActive && b.isLocal)
                {
                    random = Rando.generator;
                    Rando.generator = new Random(NetRand.currentSeed);
                }
                List<Bullet> varBullets = new List<Bullet>();
                for (int index = 0; index < 12; ++index)
                {
                    float num = (float)(index * 30f - 10f) + Rando.Float(20f);
                    ATMissileShrapnel type = new ATMissileShrapnel
                    {
                        range = 15f + Rando.Float(5f)
                    };
                    Vec2 vec2 = new Vec2((float)Math.Cos(Maths.DegToRad(num)), (float)Math.Sin(Maths.DegToRad(num)));
                    Bullet bullet = new Bullet(b.x + vec2.x * 8f, b.y - vec2.y * 8f, type, num)
                    {
                        firedFrom = b
                    };
                    varBullets.Add(bullet);
                    Level.Add(bullet);
                    if (DGRSettings.S_ParticleMultiplier != 0)
                    {
                        Level.Add(Spark.New(b.x + Rando.Float(-8f, 8f), b.y + Rando.Float(-8f, 8f), vec2 + new Vec2(Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f))));
                        Level.Add(SmallSmoke.New(b.x + vec2.x * 8f + Rando.Float(-8f, 8f), b.y + vec2.y * 8f + Rando.Float(-8f, 8f)));
                    }
                }
                if (Network.isActive && b.isLocal)
                {
                    Send.Message(new NMFireGun(null, varBullets, 0, false), NetMessagePriority.ReliableOrdered);
                    varBullets.Clear();
                }
                if (Network.isActive && b.isLocal)
                    Rando.generator = random;
                ATMissile.DestroyRadius(b.position, 75f, b);
            }
            base.OnHit(destroyed, b);
        }
    }
}
