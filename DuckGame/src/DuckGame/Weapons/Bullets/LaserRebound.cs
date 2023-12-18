namespace DuckGame
{
    public class LaserRebound : Thing
    {
        private Tex2D _rebound = Content.Load<Tex2D>("laserRebound");

        public LaserRebound(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite(_rebound);
            depth = (Depth)0.9f;
            center = new Vec2(4f, 4f);
        }

        public override void Update()
        {
            alpha -= 0.07f;
            if (alpha > 0f)
                return;
            Level.Remove(this);
        }
    }
}
