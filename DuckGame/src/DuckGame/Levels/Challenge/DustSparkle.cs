namespace DuckGame
{
    public class DustSparkle
    {
        public Vec2 position;
        public Vec2 velocity;
        public float alpha;
        public float sin;

        public DustSparkle(Vec2 pos, Vec2 vel)
        {
            position = pos;
            velocity = vel;
            sin = Rando.Float(6f);
        }
    }
}
