namespace DuckGame
{
    public class WaterParticle : Thing
    {
        public WaterParticle(float xpos, float ypos, Vec2 hitAngle)
          : base(xpos, ypos)
        {
            hSpeed = (float)(-hitAngle.x * 2 * (Rando.Float(1f) + 0.3f));
        }

        public override void Update()
        {
            vSpeed += 0.1f;
            hSpeed *= 0.9f;
            x += hSpeed;
            y += vSpeed;
            alpha -= 0.06f;
            if (alpha < 0)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw() => Graphics.DrawRect(position, position + new Vec2(1f, 1f), Color.LightBlue * alpha, depth);
    }
}
