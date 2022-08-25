using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class ShatterDuck : PhysicsParticle
    {
        public ShatterDuck(float xpos, float ypos, Sprite s, Vec2 impact = default(Vec2)) : base(xpos, ypos)
        {
            graphic = s;
            hSpeed = Rando.Float(-4, 4);
            vSpeed = Rando.Float(-1, -2);
            velocity += impact;
            _bounceEfficiency = 0.3f;
            collisionSize = new Vec2(8);
            _collisionOffset = new Vec2(-4);
            center = new Vec2(4);
        }
        public override void Update()
        {
            base.Update();
        }
    }
}
