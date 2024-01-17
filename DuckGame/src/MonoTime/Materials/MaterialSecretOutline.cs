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
            base.Apply();
        }
    }
}
