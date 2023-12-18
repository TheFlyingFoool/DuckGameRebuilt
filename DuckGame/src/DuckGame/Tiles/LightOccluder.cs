namespace DuckGame
{
    public class LightOccluder
    {
        public Vec2 p1;
        public Vec2 p2;
        public Color color;

        public LightOccluder(Vec2 p, Vec2 pp, Color c)
        {
            p1 = p;
            p2 = pp;
            color = c;
        }
    }
}
