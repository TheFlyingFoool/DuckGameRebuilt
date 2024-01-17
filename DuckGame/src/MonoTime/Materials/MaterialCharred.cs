using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialCharred : Material
    {
        public MaterialCharred()
        {
            spsupport = true;
            _effect = Content.Load<MTEffect>("Shaders/charred");
        }

        public override void Apply()
        {
            base.Apply();
        }
    }
}
