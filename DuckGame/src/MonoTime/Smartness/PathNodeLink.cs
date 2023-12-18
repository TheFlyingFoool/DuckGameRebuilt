namespace DuckGame
{
    public class PathNodeLink
    {
        public Thing link;
        public Thing owner;
        public float distance;
        public bool oneWay;
        public bool gap;

        public Vec2 position => owner.position;
    }
}
