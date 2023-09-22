using System;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class BFGBall : MaterialThing
    {
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _travelBinding = new StateBinding("travel");
        public SpriteMap sprite;
        public Vec2 travel;
        public int Existance;
        public BFGBall(float xpos, float ypos) : base(xpos, ypos)
        {
            sprite = new SpriteMap("bfgBall", 32, 32);
            sprite.AddAnimation("idle", 0.3f, true, 0, 1, 2, 3);
            sprite.SetAnimation("idle");
            graphic = sprite;
            center = new Vec2(16);
            collisionSize = new Vec2(32);
            _collisionOffset = new Vec2(-16);
        }
        public int blocksBroken;

        public void KillCircle(float size)
        {
            HashSet<ushort> hashSet = new HashSet<ushort>();
            IEnumerable<MaterialThing> mts = Level.CheckCircleAll<MaterialThing>(position, size);
            foreach (MaterialThing mt in mts)
            {
                if (mt is BlockGroup bg)
                {
                    bool wrocken = false;//kraken
                    for (int i = 0; i < bg.blocks.Count; i++)
                    {
                        AutoBlock b = (AutoBlock)bg.blocks[i];

                        if (Collision.Circle(position, size, b))
                        {
                            if (!wrocken)
                            {
                                bg.Wreck();
                                wrocken = true;
                            }
                            blocksBroken++;
                            hashSet.Add(b.blockIndex);
                            Level.current.AddUpdateOnce(b);
                            b.skipWreck = true;
                            b.shouldWreck = true;
                        }
                    }
                }
                else if (mt is AutoBlock ab)
                {
                    hashSet.Add(ab.blockIndex);
                    Level.current.AddUpdateOnce(ab);
                    ab.skipWreck = true;
                    ab.shouldWreck = true;
                    blocksBroken++;
                }
                else
                {
                    PowerfulRuleBreakingFondle(mt, DuckNetwork.localConnection);
                    mt.Destroy(new DTIncinerate(this));
                }
            }
            if (hashSet.Count > 0)
            {
                SFX.PlaySynchronized("snare", 0.7f, Rando.Float(-1.5f, -1.8f));
                Send.Message(new NMDestroyBlocks(hashSet));
            }
        }
        public override void Terminate()
        {
            float charg = 3;
            for (int i = 0; i < DGRSettings.ActualParticleMultiplier * 16; i++)
            {
                Level.Add(new DanCircParticle(x + Rando.Float(-charg * 10, charg * 10), y + Rando.Float(-charg * 10, charg * 10), charg * 8 * Rando.Float(1, 0.3f)));
            }
            base.Terminate();
        }
        public override void Update()
        {
            Existance++;
            if (isServerForObject)
            {
                if (Existance > 1800) Level.Remove(this);

                KillCircle(16);
                if (blocksBroken > 20)
                {
                    KillCircle(48);
                    SFX.PlaySynchronized("bfgExplode");
                    Level.Remove(this);
                }
                position += travel;
            }
        }
    }
}
