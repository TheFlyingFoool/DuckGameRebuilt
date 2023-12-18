namespace DuckGame
{
    public class ColorStar : PhysicsParticle, IFactory
    {
        private float maxSize;

        public ColorStar(float xpos, float ypos, Vec2 dir, Color pColor)
          : base(xpos, ypos)
        {
            graphic = new Sprite("colorStar");
            graphic.CenterOrigin();
            center = new Vec2(graphic.width / 2, graphic.height / 2);
            xscale = yscale = 0.9f;
            hSpeed = dir.x;
            vSpeed = dir.y;
            maxSize = 0.1f;
            graphic.color = pColor;
            _gravMult = 3f;
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
