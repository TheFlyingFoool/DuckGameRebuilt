namespace DuckGame
{
    public abstract class EjectedShell : PhysicsParticle
    {
        private SpriteMap _sprite;

        protected EjectedShell(float xpos, float ypos, string shellSprite, string bounceSound = "metalBounce")
          : base(xpos, ypos)
        {
            hSpeed = -4f - Rando.Float(3f);
            vSpeed = (float)-(Rando.Float(1.5f) + 1);
            _sprite = new SpriteMap(shellSprite, 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            _bounceSound = bounceSound;
            depth = (Depth)(0.3f + Rando.Float(0f, 0.1f));
        }

        public override void Update()
        {
            base.Update();
            _angle = Maths.DegToRad(-_spinAngle);
        }
    }
}
