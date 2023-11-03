using System.Collections.Generic;

namespace DuckGame
{
    public class PopEffect : Thing
    {
        private List<PopEffectPart> parts = new List<PopEffectPart>();
        private SpriteMap _sprite;

        public PopEffect(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("popLine", 16, 16);
            center = new Vec2(_sprite.w / 2, _sprite.h / 2);
            graphic = _sprite;
            int num1 = 8;
            for (int index = 0; index < num1; ++index)
            {
                float num2 = 360f / num1 * index;
                parts.Add(new PopEffectPart()
                {
                    scale = Rando.Float(0.6f, 1f),
                    rot = Maths.DegToRad(num2 + Rando.Float(-10f, 10f))
                });
            }
            scale = new Vec2(1.5f, 1.5f);
            depth = (Depth)0.85f;
        }

        public override void Update()
        {
            xscale -= 0.24f;
            yscale = xscale;
            if (xscale >= 0.01f)
                return;
            Level.Remove(this);
        }

        public override void Draw()
        {
            foreach (PopEffectPart part in parts)
            {
                _sprite.angle = part.rot;
                _sprite.xscale = _sprite.yscale = xscale * part.scale;
                _sprite.center = new Vec2(_sprite.w / 2, _sprite.h / 2);
                _sprite.alpha = 0.8f;
                Graphics.Draw(ref _sprite, x, y);
            }
            base.Draw();
        }

        public class PopEffectPart
        {
            public float scale;
            public float rot;
        }
    }
}
