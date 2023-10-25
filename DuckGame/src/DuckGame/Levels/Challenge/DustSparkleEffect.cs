using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class DustSparkleEffect : Thing
    {
        private int _sparkleWait;
        private SpriteMap _light;
        private List<DustSparkle> _sparkles = new List<DustSparkle>();
        private bool _wide;
        private bool _lit;

        public DustSparkleEffect(float xpos, float ypos, bool wide, bool lit)
          : base(xpos, ypos)
        {
            _light = !wide ? new SpriteMap("arcade/lights", 56, 57) : new SpriteMap("arcade/prizeLights", 107, 55);
            _wide = wide;
            _lit = lit;
        }

        public override void Initialize()
        {
            material = new MaterialDustSparkle(position, new Vec2(_light.width, _light.height), _wide, _lit);
            base.Initialize();
        }

        public float fade
        {
            get => (material as MaterialDustSparkle).fade;
            set => (material as MaterialDustSparkle).fade = value;
        }

        public override void Update()
        {
            for (int index = 0; index < _sparkles.Count; ++index)
            {
                DustSparkle sparkle = _sparkles[index];
                sparkle.position += sparkle.velocity;
                sparkle.position.y += (float)Math.Sin(sparkle.sin) * 0.01f;
                sparkle.sin += 0.01f;
                if (sparkle.alpha < 1f) sparkle.alpha += 0.01f;
                bool flag = false;
                if (sparkle.position.x > x + _light.width + 2f || sparkle.position.x < x - 2f || sparkle.position.y < y + 1f || sparkle.position.y > y + _light.height) flag = true;
                if (flag)
                {
                    _sparkles.RemoveAt(index);
                    index--;
                }
            }
            _sparkleWait++;
            if (_sparkleWait <= 10) return;
            _sparkleWait = 0;
            int num = 1;
            if (Rando.Float(1f) > 0.5) num = -1;
            _sparkles.Add(new DustSparkle(new Vec2(x + Rando.Float(_light.width), y + Rando.Float(_light.height)), new Vec2(Rando.Float(0.15f, 0.25f) * num, Rando.Float(-0.05f, 0.05f))));
        }

        public override void Draw()
        {
            _light.depth = depth - 2;
            _light.frame = 1;
            _light.alpha = 0.7f;
            Graphics.Draw(ref _light, x, y);
            foreach (DustSparkle sparkle in _sparkles)
                Graphics.DrawRect(sparkle.position + new Vec2(-0.5f, -0.5f), sparkle.position + new Vec2(0.5f, 0.5f), Color.White * sparkle.alpha, depth + 10);
        }
    }
}
