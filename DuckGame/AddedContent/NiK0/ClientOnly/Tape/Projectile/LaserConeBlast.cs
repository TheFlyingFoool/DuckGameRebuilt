using System.Collections.Generic;
namespace DuckGame
{
    [ClientOnly]
    public class LaserConeBlast : Thing
    {
        public Vec2 bar;
        public LaserConeBlast(float xpos, float ypos, Vec2 vec) : base(xpos, ypos)
        {
            bar = vec;
        }
        public override void Initialize()
        {
            Vec2 p1 = position;
            Vec2 p2 = position + bar * 1500;
            Vec2 up = bar.Rotate(1.57f, Vec2.Zero);
            //120
            p2 += up * 120;

            HashSet<ushort> hashSet = new HashSet<ushort>();

            for (int i = 0; i < 15; i++)
            {
                Vec2 p3 = p1 + (bar * i * 100);
                Level.Add(new LaserDiskParticle(p3.x, p3.y, new Color(180, 0, 0))
                {
                    scale = new Vec2(i * 0.7f + 1),
                    alpha = (i / 30f) + 0.7f,
                    spd = bar * i,
                    angle = angle,
                    depth = 1

                });
                /*if (i > 0)
                {
                    p3 -= bar * 40;
                    Level.Add(new LaserDiskParticle(p3.x, p3.y, Color.Red)
                    {
                        scale = new Vec2(i * 0.7f + 1) - new Vec2(0.23f),
                        alpha = (i / 30f) + 0.7f,
                        spd = bar * i,
                        angle = angle,
                        depth = 0.9f
                    });
                    p3 -= bar * 40;
                    Level.Add(new LaserDiskParticle(p3.x, p3.y, new Color(255, 70, 70))
                    {
                        scale = new Vec2(i * 0.7f + 1) - new Vec2(0.56f),
                        alpha = (i / 30f) + 0.7f,
                        spd = bar * i,
                        angle = angle,
                        depth = 0.8f
                    });
                }*/
            }
            List<BlockGroup> wreckers = new List<BlockGroup>();
            for (int i = 0; i < 24; i++)
            {

                foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(p1, p2))
                {
                    if (materialThing is AutoBlock b)
                    {
                        if (b is BlockGroup bg)
                        {
                            for (int z = 0; z < bg.blocks.Count; z++)
                            {
                                AutoBlock ab = (AutoBlock)bg.blocks[z];
                                if (ab != null && !ab.shouldWreck && Collision.Line(p1, p2, ab.rectangle))
                                {
                                    ab.shouldWreck = true;
                                    Level.current.AddUpdateOnce(ab);
                                    hashSet.Add(ab.blockIndex);
                                    Level.Add(new FadingAwayThing(ab, bar.Rotate(Rando.Float(-0.5f, 0.5f), Vec2.Zero) * Rando.Float(1.5f, 3), Rando.Float(-0.2f, 0.2f)));
                                }
                            }
                            wreckers.Add(bg);
                            //bg.Wreck();
                        }
                        else
                        {
                            if (!b.shouldWreck)
                            {
                                b.shouldWreck = true;
                                Level.current.AddUpdateOnce(b);
                                hashSet.Add(b.blockIndex);
                                Level.Add(new FadingAwayThing(b, bar.Rotate(Rando.Float(-0.5f, 0.5f), Vec2.Zero) * Rando.Float(1.5f, 3), Rando.Float(-0.2f, 0.2f)));
                            }
                        }
                    }
                    else
                    {
                        if (isLocal)
                        {
                            SuperFondle(materialThing, DuckNetwork.localConnection);
                            materialThing.Destroy(new DTIncinerate(this));
                        }
                        if (!(materialThing is IAmADuck || materialThing is Holdable || materialThing is Teleporter || materialThing is GreyBlock || materialThing.graphic == null || materialThing is IBigStupidWall))
                        {
                            Level.Remove(materialThing);
                            Level.Add(new FadingAwayThing(materialThing, bar.Rotate(Rando.Float(-0.5f, 0.5f), Vec2.Zero) * Rando.Float(1.5f, 3), Rando.Float(-0.2f, 0.2f)));
                        }
                    }
                }
                if (i % 4 == 0)
                {
                    Level.Add(new LaserLine(p1, p2 - p1, -up * ((i - 12f) / 8f), 0.4f, Color.Red, 5, 0.015f));
                }

                p2 -= up * 10;
            }
            for (int i = 0; i < wreckers.Count; i++)
            {
                wreckers[i].Wreck();
            }
            base.Initialize();
        }
    }
}
