namespace DuckGame
{
    public class RainParticel : Thing
    {
        public RainParticel(Vec2 v, float mult = 1)
        {
            lPos = v;
            position = v;
            rY = Rando.Float(6, 10);
            rX = Rando.Float(1, 2) * mult;
            Level.CheckRay<Block>(position, position + new Vec2(rX, rY) * 10000, null, out yEnd);
        }
        public override void Update()
        {
            x += rX;
            y += rY;
            if (position.y > yEnd.y)
            {
                Level.Add(new WaterSplash(position.x, yEnd.y, flud));
                Level.Remove(this);
            }
            else if (position.y > Level.current.bottomRight.y + 200) Level.Remove(this);
        }
        public override void Draw()
        {
            if (x < Level.current.camera.left || x > Level.current.camera.right)
            {
                lPos = position;
                return;
            }
            Graphics.DrawLine(lPos, position, c, 2f, 1.1f);
            lPos = position;
        }
        public Vec2 yEnd;
        public static FluidData flud = Fluid.Water;
        public static Color c = new Color(0, 112, 168);
        public float rY;
        public float rX;
        public Vec2 lPos;
    }
}
