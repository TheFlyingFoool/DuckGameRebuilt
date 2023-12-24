namespace DuckGame
{
    public class Drum : Thing
    {
        protected string _sound = "";
        protected string _alternateSound = "";
        protected float _shake;
        private SinWave _shakeWave;

        public Drum(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _shakeWave = new SinWave(this, 1.1f);
            layer = Layer.Blocks;
        }

        public void Hit()
        {
            SFX.Play(_sound, 0.9f + Rando.Float(0.1f), Rando.Float(-0.05f, 0.05f));
            _shake = 1f;
        }

        public void AlternateHit()
        {
            SFX.Play(_alternateSound, 0.9f + Rando.Float(0.1f), Rando.Float(-0.05f, 0.05f));
            _shake = 1f;
        }

        public override void Update()
        {
            _shake = Lerp.Float(_shake, 0f, 0.08f);
            base.Update();
        }

        public override void Draw()
        {
            position.x += (float)_shakeWave * _shake;
            base.Draw();
            position.x -= (float)_shakeWave * _shake;
        }
    }
}
