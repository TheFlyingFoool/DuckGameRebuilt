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
    public class ATMissile : AmmoType
    {
        public ATMissile()
        {
            accuracy = 1f;
            range = 850f;
            penetration = 0.4f;
            bulletSpeed = 7f;
            bulletThickness = 2f;
            sprite = new Sprite("missile");
            sprite.CenterOrigin();
            speedVariation = 0f;
            flawlessPipeTravel = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f))
            };
            Level.Add(pistolShell);
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
                DestroyRadius(b.position, 50f, b);
            }
            base.OnHit(destroyed, b);
        }

        public static int DestroyRadius(Vec2 pPosition, float pRadius, Thing pBullet, bool pExplode = false)
        {
            foreach (Window window in Level.CheckCircleAll<Window>(pPosition, pRadius - 20f))
            {
                Thing.Fondle(window, DuckNetwork.localConnection);
                if (Level.CheckLine<Block>(pPosition, window.position, window) == null)
                    window.Destroy(new DTImpact(pBullet));
            }
            foreach (PhysicsObject t in Level.CheckCircleAll<PhysicsObject>(pPosition, pRadius + 30f))
            {
                if (pBullet.isLocal && pBullet.owner == null)
                    Thing.Fondle(t, DuckNetwork.localConnection);
                if ((t.position - pPosition).length < 30f)
                    t.Destroy(new DTImpact(pBullet));
                t.sleeping = false;
                t.vSpeed = -2f;
            }
            //hi hello yes, pExplode is used by the positron shooter afaik and it would sometimes not destroy certain blocks
            //so i made it use the old collision if pExplode is true to fix this for now, this is probably an issue for dan to fix
            //with his collision system so its gonna stay like this for now prolly -NiK0
            if (pExplode)
            {
                int num = 0;
                HashSet<ushort> varBlocks = new HashSet<ushort>();
                foreach (BlockGroup blockGroup1 in Level.CheckCircleAllOld<BlockGroup>(pPosition, pRadius))
                {
                    if (blockGroup1 != null)
                    {
                        BlockGroup blockGroup2 = blockGroup1;
                        List<Block> blockList = new List<Block>();
                        foreach (Block block in blockGroup2.blocks)
                        {
                            if (Collision.Circle(pPosition, pRadius - 22f, block.rectangle))
                            {
                                if (!block.shouldbeinupdateloop)
                                {
                                    Level.current.AddUpdateOnce(block);
                                }
                                block.shouldWreck = true;
                                Level.current.things.UpdateObject(block);
                                if (block is AutoBlock && !(block as AutoBlock).indestructable)
                                {
                                    varBlocks.Add((block as AutoBlock).blockIndex);
                                    if (pExplode && num % 10 == 0)
                                    {
                                        Level.Add(new ExplosionPart(block.x, block.y));
                                        Level.Add(SmallFire.New(block.x, block.y, Rando.Float(-2f, 2f), Rando.Float(-2f, 2f)));
                                    }
                                    ++num;
                                }
                            }
                        }
                        blockGroup2.Wreck();
                    }
                }
                foreach (Block block in Level.CheckCircleAllOld<Block>(pPosition, pRadius - 22f))
                {
                    switch (block)
                    {
                        case AutoBlock _ when !(block as AutoBlock).indestructable:
                            block.skipWreck = true;
                            block.shouldWreck = true;
                            if (!block.shouldbeinupdateloop)
                            {
                                Level.current.AddUpdateOnce(block);
                            }
                            varBlocks.Add((block as AutoBlock).blockIndex);
                            if (pExplode)
                            {
                                if (num % 10 == 0)
                                {
                                    Level.Add(new ExplosionPart(block.x, block.y));
                                    Level.Add(SmallFire.New(block.x, block.y, Rando.Float(-2f, 2f), Rando.Float(-2f, 2f)));
                                }
                                ++num;
                                continue;
                            }
                            continue;
                        case Door _:
                        case VerticalDoor _:
                            Level.Remove(block);
                            block.Destroy(new DTRocketExplosion(null));
                            continue;
                        default:
                            continue;
                    }
                }
                if (Network.isActive && (pBullet.isLocal || pBullet.isServerForObject) && varBlocks.Count > 0)
                    Send.Message(new NMDestroyBlocks(varBlocks));
                foreach (ILight light in Level.current.things[typeof(ILight)])
                    light.Refresh();
                return varBlocks.Count;
            }
            else
            {
                HashSet<ushort> varBlocks = new HashSet<ushort>();
                foreach (BlockGroup blockGroup1 in Level.CheckCircleAll<BlockGroup>(pPosition, pRadius))
                {
                    if (blockGroup1 != null)
                    {
                        BlockGroup blockGroup2 = blockGroup1;
                        foreach (Block block in blockGroup2.blocks)
                        {
                            if (Collision.Circle(pPosition, pRadius - 22f, block.rectangle))
                            {
                                if (!block.shouldbeinupdateloop)
                                {
                                    Level.current.AddUpdateOnce(block);
                                }
                                block.shouldWreck = true;
                                if (block is AutoBlock && !(block as AutoBlock).indestructable)
                                {
                                    varBlocks.Add((block as AutoBlock).blockIndex);
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
                            if (!block.shouldbeinupdateloop)
                            {
                                Level.current.AddUpdateOnce(block);
                            }
                            varBlocks.Add((block as AutoBlock).blockIndex);
                            continue;
                        case Door _:
                        case VerticalDoor _:
                            Level.Remove(block);
                            block.Destroy(new DTRocketExplosion(null));
                            continue;
                        default:
                            continue;
                    }
                }
                if (Network.isActive && (pBullet.isLocal || pBullet.isServerForObject) && varBlocks.Count > 0)
                    Send.Message(new NMDestroyBlocks(varBlocks));
                foreach (ILight light in Level.current.things[typeof(ILight)])
                    light.Refresh();
                return varBlocks.Count;
            }
        }
    }
}
