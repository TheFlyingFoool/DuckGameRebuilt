using System;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class PortalParticle : Thing
    {
        public PortalParticle(float xpos, float ypos, Color c) : base(xpos, ypos)
        {
            graphic = new Sprite(Graphics.blankWhiteSquare);
            graphic.color = c;
        }
        public override void Update()
        {
            alpha -= 0.01f;
            scale -= new Vec2(0.01f);
            position += velocity;
            velocity = Lerp.Vec2Smooth(velocity, Vec2.Zero, 0.03f);
            if (scale.x <= 0 || alpha <= 0)
            {
                Level.Remove(this);
            }
        }
        public override void Draw()
        {
            base.Draw();
        }
    }
    [ClientOnly]
    public class LPortal : MaterialThing
    {
        public bool orange;
        public Block adjadcent;
        public Vec2 orientation;
        public LPortal(float xpos, float ypos) : base(xpos, ypos) 
        {
            collisionSize = new Vec2(24);
        }
        public void PositionOnBlock()
        {
            if (adjadcent != null)
            {
                if (orientation.x < 0)
                {
                    position = new Vec2(adjadcent.left, adjadcent.y) + orientation;
                    collisionSize = new Vec2(10, 32);
                    _collisionOffset = new Vec2(-8, -16);
                }
                else if (orientation.x > 0)
                {
                    position = new Vec2(adjadcent.right, adjadcent.y) + orientation;
                    collisionSize = new Vec2(10, 32);
                    _collisionOffset = new Vec2(-2, -16);
                }
                else if (orientation.y < 0)
                {
                    position = new Vec2(adjadcent.x, adjadcent.top) + orientation;
                    collisionSize = new Vec2(32, 10);
                    _collisionOffset = new Vec2(-16, -8);
                }
                else if (orientation.y > 0)
                {
                    position = new Vec2(adjadcent.x, adjadcent.bottom) + orientation;
                    collisionSize = new Vec2(32, 10);
                    _collisionOffset = new Vec2(-16, 0);
                }
            }
        }
        public LPortal link;
        public void TPEffect()
        {
            for (int i = 0; i < 12 * DGRSettings.ActualParticleMultiplier; i++)
            {
                PortalParticle pp = new PortalParticle(x, y, orange ? new Color(227, 171, 2) : new Color(2, 222, 206));
                pp.x = Rando.Float(left, right);
                pp.y = Rando.Float(top, bottom);
                pp.velocity = orientation * Rando.Float(0, 4);
                pp.scale = new Vec2(Rando.Float(0.8f, 2.3f));
                pp.alpha = Rando.Float(0.8f, 1.2f);
                Level.Add(pp);
            }
            otm = 5f;
        }
        public float timer;
        public override void Update()
        {
            timer += 0.08f * DGRSettings.ActualParticleMultiplier;
            if (timer > 1)
            {
                timer = 0;
                PortalParticle pp = new PortalParticle(x, y, orange ? new Color(227, 171, 2) : new Color(2, 222, 206));
                pp.x = Rando.Float(left, right);
                pp.y = Rando.Float(top, bottom);
                pp.velocity = orientation * Rando.Float(0.2f, 1.3f);
                pp.scale = new Vec2(Rando.Float(1f, 1.5f));
                pp.alpha = Rando.Float(1, 1.7f);
                Level.Add(pp);
            }

            if (link != null && link.removeFromLevel)
            {
                link = null;
            }

            otm = Lerp.FloatSmooth(otm, link != null ? 2.5f : 1.5f, 0.1f);

            ms = Maths.Clamp(Lerp.FloatSmooth(ms, 20, 0.1f), 0, 16);



            if (link != null)
            {
                IEnumerable<ITeleport> itps = Level.CheckRectAll<ITeleport>(topLeft, bottomRight);
                foreach (ITeleport it in itps)
                {
                    Thing t = (Thing)it;
                    if (t != null && t.owner == null)
                    {
                        t.position = link.position + link.orientation * 24;

                        if (t is PhysicsObject po && po.grounded && po.lastVelocity.length > po.velocity.length) 
                        {
                            po.velocity = po.lastVelocity;
                        }
                        Vec2 v = t.velocity;
                        Vec2 translated = t.velocity;
                        if (t is QuadLaserBullet qlb)
                        {
                            v = qlb.travel;
                            translated = qlb.travel;
                        }
                        //this code is quite idiotic
                        if (orientation.y != 0 || link.orientation.y != 0)
                        {
                            if (orientation.y == link.orientation.y)
                            {
                                translated.y *= -1;
                            }
                            else if (orientation.x != 0 || link.orientation.x != 0)
                            {
                                if (orientation.x != 0)
                                {
                                    translated.x = v.y;
                                    translated.y = Math.Abs(v.x) * link.orientation.y;
                                }
                                else
                                {
                                    translated.x = Math.Abs(v.y) * link.orientation.x;
                                    translated.y = v.x;
                                }
                            }
                        }
                        else if (orientation.x != 0 || link.orientation.x != 0)
                        {
                            if (orientation.x == link.orientation.x && orientation.x != 0)
                            {
                                translated.x *= -1;
                            }
                            else if (orientation.y != 0 || link.orientation.y != 0)
                            {
                                if (orientation.y != 0)
                                {
                                    translated.x = Math.Abs(v.y) * link.orientation.x;
                                    translated.y = v.x;
                                }
                                else
                                {//this has x and link has y
                                    translated.x = v.y;
                                    translated.y = Math.Abs(v.x) * link.orientation.y;
                                }
                            }
                        }

                        if (t is QuadLaserBullet qlb2)
                        {
                            qlb2.travel = translated;
                        }
                        else t.velocity = translated;
                        if (t is EnergyScimitar en && en._airFly)
                        {
                            en._airFlyAngle = Maths.PointDirection(Vec2.Zero, en.velocity);
                        }

                        t.OnTeleport();
                        link.TPEffect();
                    }
                }
            }
        }
        public float ms;
        public float otm;
        public override void Draw()
        {

            Vec2 v = orientation.Rotate(1.57f, Vec2.Zero);
            Vec2 p = position - orientation;
            for (int i = 0; i < 5; i++)
            {
                Graphics.DrawLine(p + v * ms, p - v * ms, (orange ? new Color(227, 171, 2) : new Color(2, 222, 206)) * (1f - (i / 5f)), otm + 0.1f, 0.9f);
                p += orientation * otm;
            }
        }
    }
}
