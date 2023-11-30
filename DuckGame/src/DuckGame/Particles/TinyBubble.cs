namespace DuckGame
{
    public class TinyBubble : PhysicsParticle
    {
        private SinWaveManualUpdate _wave = new SinWaveManualUpdate(0.1f + Rando.Float(0.1f), Rando.Float(3f));
        private float _minY;
        private float _waveSize = 1f;

        public TinyBubble(float xpos, float ypos, float startHSpeed, float minY, bool blue = false)
          : base(xpos, ypos)
        {
            alpha = 0.7f;
            _minY = minY;
            _gravMult = 0f;
            vSpeed = -Rando.Float(0.5f, 1f);
            hSpeed = startHSpeed;
            depth = (Depth)0.3f;
            SpriteMap spriteMap = new SpriteMap("tinyBubbles", 8, 8);
            if (blue)
                spriteMap = new SpriteMap("tinyBlueBubbles", 8, 8);
            spriteMap.frame = Rando.Int(0, 1);
            graphic = spriteMap;
            center = new Vec2(4f, 4f);
            _waveSize = Rando.Float(0.1f, 0.3f);
            xscale = yscale = 0.1f;
        }

        public override void Update()
        {
            _wave.Update();
            position.x += _wave.value * _waveSize;
            position.x += hSpeed;
            position.y += vSpeed;
            hSpeed = Lerp.Float(hSpeed, 0f, 0.1f);
            xscale = yscale = Lerp.Float(xscale, 1f, 0.1f);
            if (y < _minY - 4)
                alpha -= 0.025f;
            if (y < _minY - 8)
                alpha = 0f;
            if (y >= _minY)
                return;
            alpha -= 0.025f;
            if (alpha >= 0)
                return;
            Level.Remove(this);
        }
    }
}
