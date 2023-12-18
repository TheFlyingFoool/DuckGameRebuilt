namespace DuckGame
{
    public class Ember : PhysicsParticle
    {
        public SinWaveManualUpdate _wave = new SinWaveManualUpdate(0.1f + Rando.Float(0.1f));
        private Color _col;
        private float _initialLife = 1f;

        public Ember(float xpos, float ypos)
          : base(xpos, ypos)
        {
            vSpeed = -(0.2f + Rando.Float(0.7f));
            hSpeed = Rando.Float(0.4f) - 0.2f;
            _col = Rando.Float(1f) >= 0.4f ? (Rando.Float(1f) >= 0.4f ? Color.Gray : Color.Orange) : Color.Yellow;
            if (Rando.Float(1f) < 0.2f)
                _initialLife += Rando.Float(10f);
            alpha = 0.7f;
        }
        public bool windAffected = true;
        public override void Update()
        {
            if (GameLevel.rainwind != 0 && windAffected)
            {
                hSpeed = Lerp.Float(hSpeed, GameLevel.rainwind * 0.5f, 0.04f);
            }
            _wave.Update();
            position.x += _wave.value * 0.2f;
            position.x += hSpeed;
            position.y += vSpeed;
            _initialLife -= 0.1f;
            if (_initialLife >= 0f)
                return;
            alpha -= 0.025f;
            if (alpha >= 0f)
                return;
            Level.Remove(this);
        }

        public override void Draw() => Graphics.DrawRect(position, position + new Vec2(1f, 1f), _col * alpha, depth);
    }
}
