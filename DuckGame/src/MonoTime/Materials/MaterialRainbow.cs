using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialRainbow : Material// test
    {
        public float offset;
        public float offset2;

        public MaterialRainbow() => _effect = Content.Load<MTEffect>("Shaders/rainbow");

        public override void Apply()
        {
            _effect.effect.Parameters["offset"].SetValue(offset);
            _effect.effect.Parameters["offset2"].SetValue(offset2);
            base.Apply();
        }
    }
}
