namespace DuckGame
{
    public class DizzyStar : PhysicsParticle, IFactory
    {
        private float maxSize;

        public DizzyStar(float xpos, float ypos, Vec2 dir)
          : base(xpos, ypos)
        {
            graphic = new Sprite("dizzyStar");
            graphic.CenterOrigin();
            xscale = yscale = Rando.Float(0.7f, 1.3f);
            hSpeed = dir.x;
            vSpeed = dir.y;
            maxSize = 0.1f;
        }

        public override void Update()
        {
            xscale = Lerp.Float(xscale, maxSize, 0.04f);
            yscale = xscale;
            if (xscale <= maxSize)
                Level.Remove(this);
            base.Update();
        }
    }
}
