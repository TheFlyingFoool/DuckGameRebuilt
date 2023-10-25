namespace DuckGame
{
    public class DartShell : PhysicsParticle
    {
        private SpriteMap _sprite;
        private float _rotSpeed;
        private bool _die;

        public DartShell(float xpos, float ypos, float rotSpeed, bool flip)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("dart", 16, 16)
            {
                flipH = flip
            };
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            _bounceSound = "";
            _rotSpeed = rotSpeed;
            depth = (Depth)0.3f;
        }

        public override void Update()
        {
            base.Update();
            angle += _rotSpeed;
            if (vSpeed < 0f || _grounded) _die = true;
            if (_die) alpha -= 0.05f;
            if (alpha > 0f) return;
            Level.Remove(this);
        }
    }
}
