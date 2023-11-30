namespace DuckGame
{
    public class GlassDebris : PhysicsParticle
    {
        private SpriteMap _sprite;

        public GlassDebris(bool rotate, float xpos, float ypos, float h, float v, int f, int tint = 0)
          : base(xpos, ypos)
        {
            hSpeed = h;
            vSpeed = v;
            _sprite = new SpriteMap("windowDebris", 8, 8)
            {
                frame = Rando.Int(7),
                color = Window.windowColors[tint] * 0.6f
            };
            graphic = _sprite;
            center = new Vec2(4f, 4f);
            _bounceEfficiency = 0.3f;
            if (!rotate)
                return;
            angle -= 1.57f;
        }

        public override void Update()
        {
            alpha -= 0.01f;
            if (alpha < 0)
                Level.Remove(this);
            base.Update();
        }
    }
}
