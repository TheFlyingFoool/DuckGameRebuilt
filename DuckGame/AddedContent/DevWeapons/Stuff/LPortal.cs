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
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _orangeBinding = new StateBinding("orange");
        public StateBinding _orientationBinding = new StateBinding("orientation");

        public bool orange;
        public Block adjadcent;
        public Vec2 orientation;
        public LPortal(float xpos, float ypos) : base(xpos, ypos) 
        {
            collisionSize = new Vec2(24);
            thickness = 10000;
        }
        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (link != null)
            {
                link.TPEffect();
                Bullet.specialRebound = true;
                bullet.DoRebound(link.position + link.orientation * 24, Maths.PointDirection(Vec2.Zero, link.orientation), 0);
                Bullet.specialRebound = false;
                return true;
            }
            else return false;
        }
        public bool positioned;
        public bool PositionOnBlock()
        {
            if (adjadcent != null)
            {
                positioned = true;
                if (orientation.x < 0)
                {
                    position = new Vec2(adjadcent.left, adjadcent.y) + orientation;
                    collisionSize = new Vec2(10, 30);
                    _collisionOffset = new Vec2(-8, -15);


                    Block bb = Level.CheckLine<Block>(new Vec2(x, top), new Vec2(x, bottom));
                    if (bb != null)
                    {
                        if (bb.y < y) top = bb.bottom;
                        else bottom = bb.top;
                    }
                    else
                    {
                        if (top < adjadcent.top) top = adjadcent.top;
                        else if (bottom > adjadcent.bottom) bottom = adjadcent.bottom;
                        bb = Level.CheckLine<Block>(new Vec2(x, top), new Vec2(x, bottom));
                        if (bb != null)
                        {
                            if (bb.y < y) top = bb.bottom;
                            else bottom = bb.top;
                        }
                    }

                    if (Level.CheckLine<Block>(topRight, topRight + new Vec2(4, 0)) == null ||
                        Level.CheckLine<Block>(bottomRight, bottomRight + new Vec2(4, 0)) == null) return false;

                    collisionSize = new Vec2(10, 32);
                    _collisionOffset = new Vec2(-8, -16);

                }
                else if (orientation.x > 0)
                {
                    position = new Vec2(adjadcent.right, adjadcent.y) + orientation;
                    collisionSize = new Vec2(10, 30);
                    _collisionOffset = new Vec2(-2, -15);

                    Block bb = Level.CheckLine<Block>(new Vec2(x, top), new Vec2(x, bottom));
                    if (bb != null)
                    {
                        if (bb.y < y) top = bb.bottom;
                        else bottom = bb.top;
                    }
                    else
                    {
                        if (top < adjadcent.top) top = adjadcent.top;
                        else if (bottom > adjadcent.bottom) bottom = adjadcent.bottom;
                        bb = Level.CheckLine<Block>(new Vec2(x, top), new Vec2(x, bottom));
                        if (bb != null)
                        {
                            if (bb.y < y) top = bb.bottom;
                            else bottom = bb.top;
                        }
                    }

                    if (Level.CheckLine<Block>(topLeft - new Vec2(4, 0), topLeft) == null ||
                        Level.CheckLine<Block>(bottomLeft - new Vec2(4, 0), bottomLeft) == null) return false;

                    collisionSize = new Vec2(10, 32);
                    _collisionOffset = new Vec2(-2, -16);
                }
                else if (orientation.y < 0)
                {
                    position = new Vec2(adjadcent.x, adjadcent.top) + orientation;
                    collisionSize = new Vec2(30, 10);
                    _collisionOffset = new Vec2(-15, -8);

                    Block bb = Level.CheckLine<Block>(new Vec2(left, y), new Vec2(right, y));
                    if (bb != null)
                    {
                        if (bb.x < x) left = bb.right;
                        else right = bb.left;
                    }
                    else
                    {
                        if (left < adjadcent.left) left = adjadcent.left;
                        else if (right > adjadcent.right) right = adjadcent.right;
                        bb = Level.CheckLine<Block>(new Vec2(left, y), new Vec2(right, y));
                        if (bb != null)
                        {
                            if (bb.x < x) left = bb.right;
                            else right = bb.left;
                        }
                    }

                    if (Level.CheckLine<Block>(bottomLeft, bottomLeft + new Vec2(0, 4)) == null ||
                        Level.CheckLine<Block>(bottomRight, bottomRight + new Vec2(0, 4)) == null) return false;

                    collisionSize = new Vec2(32, 10);
                    _collisionOffset = new Vec2(-16, -8);
                }
                else if (orientation.y > 0)
                {
                    position = new Vec2(adjadcent.x, adjadcent.bottom) + orientation;
                    collisionSize = new Vec2(30, 10);
                    _collisionOffset = new Vec2(-15, 0);

                    Block bb = Level.CheckLine<Block>(new Vec2(left, y), new Vec2(right, y));
                    if (bb != null)
                    {
                        if (bb.x < x) left = bb.right;
                        else right = bb.left;
                    }
                    else
                    {
                        if (left < adjadcent.left) left = adjadcent.left;
                        else if (right > adjadcent.right) right = adjadcent.right;
                        bb = Level.CheckLine<Block>(new Vec2(left, y), new Vec2(right, y));
                        if (bb != null)
                        {
                            if (bb.x < x) left = bb.right;
                            else right = bb.left;
                        }
                    }

                    if (Level.CheckLine<Block>(topLeft - new Vec2(0, 4), topLeft) == null ||
                        Level.CheckLine<Block>(topRight - new Vec2(0, 4), topRight) == null) return false;

                    collisionSize = new Vec2(32, 10);
                    _collisionOffset = new Vec2(-16, 0);
                }
                return true;
            }
            return false;
        }
        public LPortal link;

        public void TPEffect()
        {
            for (int i = 0; i < 10 * DGRSettings.ActualParticleMultiplier; i++)
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
            if (isServerForObject && (adjadcent == null || adjadcent.removeFromLevel)) Level.Remove(this);
            if (!isServerForObject && !positioned && orientation != Vec2.Zero)
            {
                positioned = true;
                if (orientation.x < 0)
                {
                    collisionSize = new Vec2(10, 32);
                    _collisionOffset = new Vec2(-8, -16);
                }
                else if (orientation.x > 0)
                {
                    collisionSize = new Vec2(10, 32);
                    _collisionOffset = new Vec2(-2, -16);
                }
                else if (orientation.y < 0)
                {
                    collisionSize = new Vec2(32, 10);
                    _collisionOffset = new Vec2(-16, -8);
                }
                else if (orientation.y > 0)
                {
                    collisionSize = new Vec2(32, 10);
                    _collisionOffset = new Vec2(-16, 0);
                }
            }
            /*
             * netCollision = 1;
                    position = new Vec2(adjadcent.left, adjadcent.y) + orientation;
                    collisionSize = new Vec2(10, 32);
                    _collisionOffset = new Vec2(-8, -16);
                }
                else if (orientation.x > 0)
                {
                    netCollision = 2;
                    position = new Vec2(adjadcent.right, adjadcent.y) + orientation;
                    collisionSize = new Vec2(10, 32);
                    _collisionOffset = new Vec2(-2, -16);
                }
                else if (orientation.y < 0)
                {
                    netCollision = 3;
                    position = new Vec2(adjadcent.x, adjadcent.top) + orientation;
                    collisionSize = new Vec2(32, 10);
                    _collisionOffset = new Vec2(-16, -8);
                }
                else if (orientation.y > 0)
                {
                    netCollision = 4;
                    position = new Vec2(adjadcent.x, adjadcent.bottom) + orientation;
                    collisionSize = new Vec2(32, 10);
                    _collisionOffset = new Vec2(-16, 0);
            */
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
                    if (t != null && t.owner == null && t.isServerForObject && (t is not IcerIcyBlock ic || ic.xscale < 1.5f))
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

                        Send.Message(new NMPortalEffect(link));
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
