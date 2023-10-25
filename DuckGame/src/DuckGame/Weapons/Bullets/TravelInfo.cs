namespace DuckGame
{
    public class TravelInfo
    {
        public Vec2 p1;
        public Vec2 p2;
        public float length;

        public TravelInfo(Vec2 point1, Vec2 point2, float len)
        {
            p1 = point1;
            p2 = point2;
            length = len;
        }
    }
}
