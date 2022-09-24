using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
    public class NMPinkExplode : NMEvent
    {
        public NMPinkExplode(Vec2 vec)
        {
            v = vec;
        }
        public NMPinkExplode()
        {
        }
        public Vec2 v;

        public override void Activate()
        {
            SFX.Play("explode");
            PinkBox.ExplodeEffect(v);
            List<IAmADuck> physicsObjects = Level.CheckCircleAll<IAmADuck>(v, 128).ToList();
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                MaterialThing po = (MaterialThing)physicsObjects[i];
                Vec2 travel = Maths.AngleToVec(Maths.DegToRad(-Maths.PointDirection(v, po.position)));
                travel.x *= 10;
                travel.y *= -10;
                if (po.isServerForObject)
                {
                    po.velocity = travel / ((Extensions.Distance(v, po.position) / 50) + 0.01f);
                    if (po is Duck d)
                    {
                        d.Disarm(null);
                    }
                }
            }
        }
    }
}
