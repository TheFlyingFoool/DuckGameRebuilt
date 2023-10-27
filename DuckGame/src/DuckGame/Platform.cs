namespace DuckGame
{
    public class Platform : MaterialThing, IPlatform
    {
        public Platform(float x, float y)
          : base(x, y)
        {
            collisionSize = new Vec2(16f, 16f);
            thickness = 10f;
        }

        public Platform(float x, float y, float wid, float hi)
          : base(x, y)
        {
            collisionSize = new Vec2(wid, hi);
            thickness = 10f;
        }
    }
}
