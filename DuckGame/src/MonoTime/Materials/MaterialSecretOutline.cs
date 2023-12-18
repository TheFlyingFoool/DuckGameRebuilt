using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialSecretOutline : Material
    {
        public MaterialSecretOutline()
        {
            spsupport = true;
            _effect = Content.Load<MTEffect>("Shaders/secret_outline");
        }

        public override void Apply()
        {
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
