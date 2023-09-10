using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [ClientOnly]
    public class Coin : PhysicsObject
    {
        public Coin(float xpos, float ypos) : base(xpos, ypos) 
        {
            graphic = new Sprite("coin");
            center = new Vec2(1.5f, 2.5f);
            collisionSize = new Vec2(10, 10);
            _collisionOffset = new Vec2(-5, -5);
            airFrictionMult = 0;
            gravMultiplier = 0.9f;
            weight = 0;
            scale = new Vec2(2);
            thickness = 200;
        }
        public int frames;
        public bool used;
        public List<Vec2> trail = new List<Vec2>();
        public StateBinding _framesBinding = new StateBinding("frames");
        public StateBinding _alphaBinding = new StateBinding("alpha");
        public StateBinding _usedBinding = new StateBinding("used");

        public List<Vec2> TargetNear(Duck ignore, bool targetCoins = false, bool ignoreWalls = false)
        {
            List<Vec2> list = new List<Vec2>();
            used = true;

            IEnumerable<Thing> ccs = Level.current.things[typeof(Coin)];
            if (ccs.Count() > 1 && targetCoins)
            {
                float nearest2 = 1239123;
                Thing theOne2 = null;
                Thing theTwo2 = null;

                foreach (Thing t in ccs)
                {
                    float ff = (position - t.position).length;
                    if (t != this && ff < nearest2 && !((Coin)t).used && ((Coin)t).frames > 5 && (ignoreWalls || Level.CheckLine<Block>(position, t.position) == null))
                    {
                        if (theTwo2 != theOne2) theTwo2 = theOne2;
                        theOne2 = t;
                        nearest2 = ff;
                    }
                }
                if (theOne2 != null)
                {
                    list.Add(theOne2.position);
                    if (theTwo2 != null && ((frames > 20 && frames < 26) || frames > 60))
                    {
                        list.Add(theTwo2.position);
                    }
                    return list;
                }
            }
            IEnumerable<Thing> dds = Level.current.things[typeof(IAmADuck)];
            float nearest = 1239123;
            Duck theOne = null;
            Duck theTwo = null;

            foreach (Thing t in dds)
            {
                Duck d = Duck.GetAssociatedDuck(t);
                float ff = (position - t.position).length;
                if (!d.dead && ff < nearest && d != ignore && (ignoreWalls || Level.CheckLine<Block>(position, t.position) == null))
                {
                    if (theTwo != theOne) theTwo = theOne;
                    theOne = d;
                    nearest = ff;
                }
            }

            if (theOne != null)
            {
                list.Add(theOne.GetPos());
                if (theTwo != null && ((frames > 20 && frames < 26) || frames > 60))
                {
                    list.Add(theTwo.GetPos());
                }
            }

            if (list.Count == 0)
            {
                Vec2 ang = Maths.AngleToVec(Rando.Float(7));
                Vec2 p2 = position + ang * 2000;
                Level.CheckRay<Block>(position, p2, null, out p2);

                list.Add(p2);
            }
            if (list.Count == 1 && ((frames > 20 && frames < 26) || frames > 60))
            {
                Vec2 ang = Maths.AngleToVec(Rando.Float(7));
                Vec2 p2 = position + ang * 2000;
                Level.CheckRay<Block>(position, p2, null, out p2);

                list.Add(p2);
            }
            return list;
        }
        public override void Update()
        {
            float siz = Maths.Clamp(frames / 4f + 10, 15, 30);
            if (used || grounded) siz = 2;
            collisionSize = new Vec2(siz);
            _collisionOffset = new Vec2(siz / -2f);
            if (isServerForObject)
            {
                frames++;
                if (frames > 60) gravMultiplier = Lerp.Float(gravMultiplier, 1.4f, 0.05f);
                if (frames > 600 || (used && !grounded)) Level.Remove(this);
                if (grounded || used)
                {
                    angle = 1.57f;
                    alpha -= 0.05f;
                    if (collideBottom != null)
                    {
                        used = true;
                        y = collideBottom.top - 3;
                        enablePhysics = false;
                    }
                    if (alpha <= 0) Level.Remove(this);
                }
                else angleDegrees += hSpeed * 4;
            }
            trail.Add(position);
            if (trail.Count > 10) trail.RemoveAt(0);
            base.Update();
        }
        public override void Draw()
        {
            for (int i = 1; i < trail.Count; i++)
            {
                Graphics.DrawLine(trail[i - 1], trail[i], Color.Yellow * ((float)i / (float)trail.Count), 5 * ((float)i / (float)trail.Count), depth - 2);
            }
            base.Draw();
        }
        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            Duck d = null;
            if (bullet.owner != null) d = (Duck)bullet.owner;
            Vec2 v = TargetNear(d, true)[0];

            Bullet.coinRebound = true;
            bullet.DoRebound(position, Maths.PointDirection(position, v), 0);
            Bullet.coinRebound = false;
            return true;
        }
    }
}
