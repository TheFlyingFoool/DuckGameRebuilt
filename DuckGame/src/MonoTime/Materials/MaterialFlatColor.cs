using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialFlatColor : Material // test
    {
        public MaterialFlatColor() => _effect = Content.Load<MTEffect>("Shaders/flatColor");

        public override void Apply()
        {
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
