using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class HitscanBullet : Thing
    {
        public Vec2 pos2;
        public Duck ignore;
        public HitscanBullet(float xpos, float ypos, Vec2 end) : base(xpos, ypos)
        {
            pos2 = end;
            bulWidth = 2.5f;
            shouldbegraphicculled = false;
        }
        public bool teled;
        public Teleporter spawnIn;
        public List<Vec2> vvs;
        public override void Initialize()
        {
            if (isLocal)
            {
                Send.Message(new NMHitscanBullet(this));
            }
            base.Initialize();
        }
        public void spawnLogic()
        {
            travelAng = Maths.PointDirectionRad(position, pos2);
            Level.CheckRay<Block>(Lerp.Vec2(position, pos2, 16), pos2, null, out pos2);
            Teleporter tele = Level.CheckRay<Teleporter>(Lerp.Vec2(position, pos2, 16), pos2, null, out pos2);
            Coin c;
            if (tele != null && tele._link != null)
            {
                c = Level.CheckRay<Coin>(position, pos2);
                if ((position - tele.position).length > 16 && c == null)
                {

                    pos2 = Collision.LineIntersectPoint(position, pos2, new Vec2(tele.x, tele.bottom), new Vec2(tele.x, tele.top));

                    spawnIn = tele;

                }
            }
            else c = Level.CheckRay<Coin>(position, pos2);
            
            if (c != null)
            {
                pos2 = c.position;
                vvs = c.TargetNear(ignore, true, false, true);
            }
        }
        public float travelAng;
        public Color c = Color.White;
        public override void Update()
        {
            if (spawnIn != null && !teled)
            {
                teled = true;

                Vec2 v2 = spawnIn.position + Maths.AngleToVec(travelAng + Rando.Float(-0.1f, 0.1f)) * 4000;

                Vec2 relative = pos2 - spawnIn.position;


                HitscanBullet hsc = new HitscanBullet(spawnIn._link.x + relative.x, spawnIn._link.y + relative.y, v2);
                hsc.spawnLogic();
                Level.Add(hsc);

                IEnumerable<MaterialThing> mts = Level.CheckLineAll<MaterialThing>(hsc.position, hsc.pos2);
                foreach (MaterialThing mt in mts)
                {
                    if (mt is IAmADuck)
                    {
                        SuperFondle(mt, DuckNetwork.localConnection);
                        mt.Destroy(new DTShot(null));
                    }
                    else
                    {
                        Fondle(mt);
                        mt.Hurt(0.1f);
                    }
                }
            }
            if (vvs != null && vvs.Count > 0)
            {
                HitscanBullet hsc = new HitscanBullet(pos2.x, pos2.y, vvs[0]);
                hsc.c = Color.Yellow;
                hsc.spawnLogic();
                hsc.bulWidth = 3.5f;
                hsc.ignore = ignore;
                Level.Add(hsc);

                IEnumerable<MaterialThing> mts = Level.CheckLineAll<MaterialThing>(hsc.position, hsc.pos2);
                foreach (MaterialThing mt in mts)
                {
                    if (mt == ignore) continue;
                    if (mt is IAmADuck)
                    {
                        SuperFondle(mt, DuckNetwork.localConnection);
                        mt.Destroy(new DTShot(null));
                    }
                    else
                    {
                        Fondle(mt);
                        mt.Hurt(0.1f);
                    }
                }
                vvs.RemoveAt(0);
                SFX.PlaySynchronized("targetRebound", 1, Rando.Float(-0.1f, 0.2f));

            }
            if (bulWidth <= 0)
            {
                Level.Remove(this);
            }
        }
        public float bulWidth;
        public override void Draw()
        {
            if (MonoMain.UpdateLerpState) bulWidth -= 0.5f;
            Graphics.DrawLine(position, pos2, c, bulWidth, 1);
            base.Draw();
        }
    }
}
