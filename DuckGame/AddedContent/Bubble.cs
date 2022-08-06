using System.Linq;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class Bubble : MaterialThing
    {
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _scaleBinding = new StateBinding("scale");
        public StateBinding _alphaBinding = new StateBinding("alpha");
        public Bubble(float xpos, float ypos, Sprite b) : base(xpos, ypos)
        {
            graphic = b;
            scale = new Vec2(Rando.Float(0.5f, 1f));
            alpha = 2;
            depth = 1;
        }
        
        public override void Update()
        {
            collisionSize = new Vec2(9 * xscale);
            _collisionOffset = new Vec2(-4.5f * xscale);
            if (alpha > 0.5f)
            {
                List<PhysicsObject> pos = Level.CheckCircleAll<PhysicsObject>(position, 10).ToList();
                for (int i = 0; i < pos.Count; i++)
                {
                    PhysicsObject po = pos[i];
                    if (po.isServerForObject)
                    {
                        po.specialFrictionMod = 0.16f;
                        po.modFric = true;
                    }
                }
            }
            if (isServerForObject)
            {
                y -= alpha * 0.05f;
                alpha -= 0.01f;
                if (alpha <= 0)
                {
                    Level.Remove(this);
                }
            }
            base.Update();
        }
    }
}
