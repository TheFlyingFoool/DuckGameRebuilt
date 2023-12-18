namespace DuckGame
{
    public class RainParticel : Thing
    {
        public static SpriteMap splash;
        public RainParticel(Vec2 v, float mult = 1)
        {
            if (splash == null)
            {
                splash = new SpriteMap("rainSplash", 8, 8);
            }
            position = v;
            rY = Rando.Float(6, 10);
            rX = Rando.Float(1, 2) * mult;
            Level.CheckRay<Block>(position, position + new Vec2(rX, rY) * 10000, null, out yEnd);
        }
        public override void DoUpdate()
        {
            Update();
            if (Buckets.Length > 0 && ((oldcollisionOffset != collisionOffset || oldcollisionSize != collisionSize) || (oldposition - position).LengthSquared() > 50f) && Level.current != null) //((oldposition - position)).length > 10
            {
                oldcollisionOffset = collisionOffset;
                oldcollisionSize = collisionSize;
                oldposition = position;
                Level.current.things.UpdateObject(this);
            }
        }
        public override void Update()
        {
            if (!die)
            {
                x += rX;
                y += rY;
                if (position.y > yEnd.y)
                {
                    die = true;
                    y = yEnd.y;
                }
                else if (position.y > Level.current.bottomRight.y + 200) Level.Remove(this);
            }
            else
            {
                sub++;
                if (splashFrame < 5 && sub % 4 == 0) splashFrame++;
                
                alpha -= 0.05f;
                if (alpha <= 0) Level.Remove(this);
            }
        }
        public int sub;
        public int splashFrame;
        public override void Draw()
        {
            if (die)
            {
                splash.alpha = alpha;
                splash.frame = splashFrame;
                splash.color = Color.White * 0.8f;
                Graphics.Draw(splash, x, y - 9, -1);
            }
            else Graphics.DrawLine(position - new Vec2(rX, rY), position, c, 1f, -1);
        }
        public bool die;
        public Vec2 yEnd;
        public static FluidData flud = Fluid.Water;
        public static Color c = Color.White * 0.8f;
        public float rY;
        public float rX;
    }
}
