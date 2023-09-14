using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class OthProjectile : PhysicsObject
    {
        public OthProjectile(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("othProj");
            center = new Vec2(8);
            bouncy = 1;
            gravMultiplier = 0;
            friction = 0;
            airFrictionMult = 0;

            collisionSize = new Vec2(16);
            _collisionOffset = new Vec2(-8);

            weight = 0;
            depth = 1;
        }
        public List<Vec2> trails = new List<Vec2>();
        public float time;
        public override void Draw()
        {
            Vec2 v = position;

            Vec2 zg = new Vec2((float)Math.Sin(time), (float)Math.Cos(time));

            trails.Add(zg * Rando.Float(8, 18));
            trails.Add(zg * Rando.Float(8, 18));

            if (trails.Count > 14)
            {
                trails.RemoveAt(0);
                trails.RemoveAt(0);
            }

            if (trails.Count > 4)
            {
                float alph = 0;
                for (int i = 2; i < trails.Count; i += 2)
                {
                    alph += 0.14f * alpha;
                    Color c = DGRDevs.Othello7.Color * alph;
                    //Graphics.DrawLine(v + trails[i - 3], v + trails[i + 2], c, 1, 1);
                    Graphics.DrawLine(v + trails[i - 2] * xscale, v + trails[i + 1] * xscale, c, 1, 1);
                    Graphics.DrawLine(v + trails[i - 1] * xscale, v + trails[i] * xscale, c, 1, 1);
                }
            }

            //Graphics.DrawLine(v + new Vec2((float)Math.Sin(time), (float)Math.Cos(time)) * 12 , v, DGRDevs.Othello7.Color, 1, depth);
            x += Rando.Float(-1, 1);
            y += Rando.Float(-1, 1);
            base.Draw();
            position = v;
        }
        public bool BOOM;
        public int realTimer;
        public override void Update()
        {
            time += Rando.Float(0.3f, 0.5f);
            if (isServerForObject)
            {
                realTimer++;
                if (realTimer > 240 || BOOM)
                {
                    scale += new Vec2(0.1f);
                    alpha -= 0.05f;
                    if (alpha <= 0)
                    {
                        Level.Remove(this);
                    }

                }
                if (alpha > 0.3f && !BOOM && realTimer > 4)
                {
                    IEnumerable<IAmADuck> iaads = Level.CheckCircleAll<IAmADuck>(position, 16 * xscale);
                    foreach (IAmADuck iaad in iaads)
                    {
                        MaterialThing mt = (MaterialThing)iaad;
                        Duck d = Duck.GetAssociatedDuck(mt);
                        if (d != null && !d.dead)
                        {
                            BOOM = true;
                            if (d.isServerForObject)
                            {
                                DodgeblockCurse dc = new DodgeblockCurse((float)Math.Round(x / 16) * 16, (float)Math.Round(y / 16) * 16);
                                dc.duck = d;
                                Level.Add(dc);
                            }
                            else
                            {
                                Send.Message(new NMDodgeBlockCurse(d));
                            }
                            break;
                        }
                    }
                }
            }
            base.Update();
        }
    }
}
