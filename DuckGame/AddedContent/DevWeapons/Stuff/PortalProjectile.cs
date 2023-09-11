using System.Collections.Generic;

namespace DuckGame
{
    public class PortalProjectile : MaterialThing, ITeleport
    {
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _velocityBinding = new StateBinding("velocity");
        public StateBinding _frameBinding = new StateBinding("frame");
        public SpriteMap sprite;
        public PortalProjectile(float xpos, float ypos) : base(xpos, ypos)
        {
            sprite = new SpriteMap("portalBullet", 13, 8);
            graphic = sprite;
            center = new Vec2(6.5f, 4f);
            collisionSize = new Vec2(8);
            _collisionOffset = new Vec2(-4);
        }
        public LutalliGun firedBy;
        public List<Vec2> trail = new List<Vec2>();
        public int life;
        public override void OnTeleport()
        {
            trail.Clear();
            base.OnTeleport();
        }
        public override void Update()
        {
            angle = -Maths.PointDirectionRad(Vec2.Zero, velocity);

            if (sprite.imageIndex == 0) c = new Color(2, 222, 206);
            else c = new Color(227, 171, 2);

            if (isServerForObject)
            {
                life++;
                if (life > 300) Level.Remove(this);

                Vec2 v = position;
                position += velocity;
                Vec2 v2 = position;
                Block block = Level.CheckRay<Block>(v, position, out position);
                if (block != null)
                {
                    bool explode = false;
                    if (block is BlockGroup bg)
                    {
                        bool stop = false;
                        for (int i = 0; i < bg.blocks.Count; i++)
                        {
                            if (Collision.Line(v, v2, bg.blocks[i]))
                            {
                                stop = true;
                                block = bg.blocks[i];
                                break;
                            }
                        }
                        if (!stop)
                        {
                            explode = true;
                        }
                    }
                    if (firedBy != null && !explode)
                    {
                        LPortal lportal = new LPortal(x, y);
                        lportal.adjadcent = block;
                        lportal.orange = sprite.imageIndex == 1;

                        position -= velocity.normalized;

                        if (x < block.left && Level.CheckPoint<Block>(block.position - new Vec2(12, 0)) == null)
                        {
                            lportal.orientation = new Vec2(-1, 0);
                        }
                        else if (x > block.right && Level.CheckPoint<Block>(block.position + new Vec2(12, 0)) == null)
                        {
                            lportal.orientation = new Vec2(1, 0);

                        }
                        else if (y < block.top && Level.CheckPoint<Block>(block.position - new Vec2(0, 12)) == null)
                        {
                            lportal.orientation = new Vec2(0, -1);

                        }
                        else if (y > block.bottom && Level.CheckPoint<Block>(block.position + new Vec2(0, 12)) == null)
                        {
                            lportal.orientation = new Vec2(0, 1);
                        }
                        else
                        {
                            explode = true;
                        }
                        if (!explode)
                        {
                            SFX.Play("scimiSurge");
                            lportal.PositionOnBlock();
                            if (Level.CheckRect<LPortal>(lportal.topLeft, lportal.bottomRight, lportal) == null)
                            {
                                Level.Add(lportal);

                                if (sprite.imageIndex == 0)
                                {
                                    if (firedBy.portal1 != null)
                                    {
                                        Fondle(firedBy.portal1);
                                        Level.Remove(firedBy.portal1);
                                    }
                                    firedBy.portal1 = lportal;
                                }
                                else
                                {
                                    if (firedBy.portal2 != null)
                                    {
                                        Fondle(firedBy.portal2);
                                        Level.Remove(firedBy.portal2);
                                    }
                                    firedBy.portal2 = lportal;
                                }
                            }
                            else explode = true;
                        }
                    }
                    else explode = true;

                    if (explode)
                    { 
                    }
                    Level.Remove(this);
                }
            }


            trail.Add(position);
            if (trail.Count > 15) trail.RemoveAt(0);
        }
        public Color c;
        public override void Draw()
        {
            for (int i = 1; i < trail.Count; i++)
            {
                Graphics.DrawLine(trail[i - 1], trail[i], c * ((float)i / (float)trail.Count), 5 * (i / (float)trail.Count), depth - 2);
            }
            base.Draw();
        }
    }
}
