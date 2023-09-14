using System.Collections.Generic;

namespace DuckGame
{
    public class InkSpike : MaterialThing
    {
        public Vec2 vector;
        public Vec2 end;

        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _endBinding = new StateBinding("end");
        public InkSpike(float xpos, float ypos, Vec2 vec) : base(xpos, ypos)
        {
            vector = position + vec * 96;
            end = position;
            for (int i = 0; i < 16; i++)
            {
                InvisibleBlock inv = new InvisibleBlock(x, y, 4, 4);
                iis.Add(inv);
                Level.Add(inv);
            }
        }
        public override void Terminate()
        {
            for (int i = 0; i < iis.Count; i++)
            {
                Level.Remove(iis[i]);
            }
            base.Terminate();
        }
        public int existance;
        public List<Duck> drag = new List<Duck>();
        public override void Update()
        {
            if (isServerForObject)
            {
                for (int i = 0; i < drag.Count; i++)
                {
                    Duck d = drag[i];//THE DUCK IN THE LIST!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    PowerfulRuleBreakingFondle(d, DuckNetwork.localConnection);//BREAK ALL THE RULES!!!!!!!!!!!!!!!!!!!!!
                    if (d.ragdoll != null && (d.ragdoll.part2.position - end).length > 8)//CHECK IF THE RAGDOLL IS NOT NULL!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    {
                        d.ragdoll.part1.position = end; //BAD CODE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        d.ragdoll.part2.position = end;
                        d.ragdoll.part3.position = end;
                        d.ragdoll.position = end;
                    }
                }                
                existance++;
                if (existance < 30)
                {
                    end = Lerp.Vec2Smooth(end, vector, 0.06f);
                    IEnumerable<IAmADuck> iaads = Level.CheckLineAll<IAmADuck>(position, end);
                    foreach (IAmADuck iaad in iaads)
                    {
                        MaterialThing mt = (MaterialThing)iaad;
                        Duck d = Duck.GetAssociatedDuck(mt);
                        if (d != null && (!d.dead || !drag.Contains(d)))
                        {
                            d.Kill(new DTImpale(this));
                            drag.Add(d);
                        }
                    }
                }
                else if (existance > 200)
                {
                    end = Lerp.Vec2Smooth(end, position, 0.06f);
                    if (existance > 230) Level.Remove(this);
                }
            }
            Vec2 v = end;
            for (int i = 0; i < 16; i++)
            {
                InvisibleBlock z = iis[i];
                z.position = v;
                z.collisionSize = new Vec2(i + 1);
                z.collisionOffset = z.collisionSize * -0.5f;
                z.thickness = i;
                z.solid = true;

                v = Lerp.Vec2(v, position, 5.3f);
            }

            base.Update();
        }
        public List<InvisibleBlock> iis = new List<InvisibleBlock>();
        public override void Draw()
        {
            Vec2 v = end;
            for (int i = 0; i < 8; i++)
            {
                if (i == 0) Graphics.DrawLine(v, position, DGRDevs.Firebreak.Color, 1, 0.8f);
                else Graphics.DrawLine(v, position, DGRDevs.Firebreak.Color, i + 2, 0.8f);
                v = Lerp.Vec2Smooth(v, position, 0.16f);
                Graphics.DrawLine(v, position, Color.Black, i + 1, 0.9f);
            }
            base.Draw();
        }
    }
}
