namespace DuckGame
{
    public class GlassParticle : PhysicsParticle
    {
        private int _tint;

        public GlassParticle(float xpos, float ypos, Vec2 hitAngle, int tint = -1)
          : base(xpos, ypos)
        {
            hSpeed = -hitAngle.x * 2f * (Rando.Float(1f) + 0.3f);
            vSpeed = (-hitAngle.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
            _bounceEfficiency = 0.6f;
            _tint = tint;
        }

        public override void Update()
        {
            alpha -= 0.01f;
            if (alpha < 0f)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            ParticleLerp.UpdateLerpState(position, MonoMain.IntraTick, MonoMain.UpdateLerpState);

            Graphics.DrawRect(ParticleLerp.Position, ParticleLerp.Position + new Vec2(1f, 1f), (_tint > 0 ? Window.windowColors[_tint] : Color.LightBlue) * alpha, depth);
        }
    }
}
