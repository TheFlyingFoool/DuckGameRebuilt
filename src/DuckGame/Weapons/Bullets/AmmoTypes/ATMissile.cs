// Decompiled with JetBrains decompiler
// Type: DuckGame.ATMissile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class ATMissile : AmmoType
    {
        public ATMissile()
        {
            this.accuracy = 1f;
            this.range = 850f;
            this.penetration = 0.4f;
            this.bulletSpeed = 7f;
            this.bulletThickness = 2f;
            this.sprite = new Sprite("missile");
            this.sprite.CenterOrigin();
            this.speedVariation = 0.0f;
            this.flawlessPipeTravel = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y);
            pistolShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)pistolShell);
        }

        public override void OnHit(bool destroyed, Bullet b)
        {
            if (!b.isLocal)
                return;
            if (destroyed)
            {
                RumbleManager.AddRumbleEvent(b.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
                new ATMissileShrapnel().MakeNetEffect(b.position, false);
                Random random = (Random)null;
                if (Network.isActive && b.isLocal)
                {
                    random = Rando.generator;
                    Rando.generator = new Random(NetRand.currentSeed);
                }
                List<Bullet> varBullets = new List<Bullet>();
                for (int index = 0; index < 12; ++index)
                {
                    float num = (float)((double)index * 30.0 - 10.0) + Rando.Float(20f);
                    ATMissileShrapnel type = new ATMissileShrapnel();
                    type.range = 15f + Rando.Float(5f);
                    Vec2 vec2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(num)), (float)Math.Sin((double)Maths.DegToRad(num)));
                    Bullet bullet = new Bullet(b.x + vec2.x * 8f, b.y - vec2.y * 8f, (AmmoType)type, num);
                    bullet.firedFrom = (Thing)b;
                    varBullets.Add(bullet);
                    Level.Add((Thing)bullet);
                    Level.Add((Thing)Spark.New(b.x + Rando.Float(-8f, 8f), b.y + Rando.Float(-8f, 8f), vec2 + new Vec2(Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f))));
                    Level.Add((Thing)SmallSmoke.New(b.x + vec2.x * 8f + Rando.Float(-8f, 8f), b.y + vec2.y * 8f + Rando.Float(-8f, 8f)));
                }
                if (Network.isActive && b.isLocal)
                {
                    Send.Message((NetMessage)new NMFireGun((Gun)null, varBullets, (byte)0, false), NetMessagePriority.ReliableOrdered);
                    varBullets.Clear();
                }
                if (Network.isActive && b.isLocal)
                    Rando.generator = random;
                ATMissile.DestroyRadius(b.position, 50f, (Thing)b);
            }
            base.OnHit(destroyed, b);
        }

        public static int DestroyRadius(Vec2 pPosition, float pRadius, Thing pBullet, bool pExplode = false)
        {
            foreach (Window window in Level.CheckCircleAll<Window>(pPosition, pRadius - 20f))
            {
                Thing.Fondle((Thing)window, DuckNetwork.localConnection);
                if (Level.CheckLine<Block>(pPosition, window.position, (Thing)window) == null)
                    window.Destroy((DestroyType)new DTImpact(pBullet));
            }
            foreach (PhysicsObject t in Level.CheckCircleAll<PhysicsObject>(pPosition, pRadius + 30f))
            {
                if (pBullet.isLocal && pBullet.owner == null)
                    Thing.Fondle((Thing)t, DuckNetwork.localConnection);
                if ((double)(t.position - pPosition).length < 30.0)
                    t.Destroy((DestroyType)new DTImpact(pBullet));
                t.sleeping = false;
                t.vSpeed = -2f;
            }
            int num = 0;
            HashSet<ushort> varBlocks = new HashSet<ushort>();
            foreach (BlockGroup blockGroup1 in Level.CheckCircleAll<BlockGroup>(pPosition, pRadius))
            {
                if (blockGroup1 != null)
                {
                    BlockGroup blockGroup2 = blockGroup1;
                    List<Block> blockList = new List<Block>();
                    foreach (Block block in blockGroup2.blocks)
                    {
                        if (Collision.Circle(pPosition, pRadius - 22f, block.rectangle))
                        {
                            block.shouldWreck = true;
                            if (block is AutoBlock && !(block as AutoBlock).indestructable)
                            {
                                varBlocks.Add((block as AutoBlock).blockIndex);
                                if (pExplode && num % 10 == 0)
                                {
                                    Level.Add((Thing)new ExplosionPart(block.x, block.y));
                                    Level.Add((Thing)SmallFire.New(block.x, block.y, Rando.Float(-2f, 2f), Rando.Float(-2f, 2f)));
                                }
                                ++num;
                            }
                        }
                    }
                    blockGroup2.Wreck();
                }
            }
            foreach (Block block in Level.CheckCircleAll<Block>(pPosition, pRadius - 22f))
            {
                switch (block)
                {
                    case AutoBlock _ when !(block as AutoBlock).indestructable:
                        block.skipWreck = true;
                        block.shouldWreck = true;
                        varBlocks.Add((block as AutoBlock).blockIndex);
                        if (pExplode)
                        {
                            if (num % 10 == 0)
                            {
                                Level.Add((Thing)new ExplosionPart(block.x, block.y));
                                Level.Add((Thing)SmallFire.New(block.x, block.y, Rando.Float(-2f, 2f), Rando.Float(-2f, 2f)));
                            }
                            ++num;
                            continue;
                        }
                        continue;
                    case Door _:
                    case VerticalDoor _:
                        Level.Remove((Thing)block);
                        block.Destroy((DestroyType)new DTRocketExplosion((Thing)null));
                        continue;
                    default:
                        continue;
                }
            }
            if (Network.isActive && (pBullet.isLocal || pBullet.isServerForObject) && varBlocks.Count > 0)
                Send.Message((NetMessage)new NMDestroyBlocks(varBlocks));
            foreach (ILight light in Level.current.things[typeof(ILight)])
                light.Refresh();
            return varBlocks.Count;
        }
    }
}
