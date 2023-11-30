using Microsoft.Xna.Framework;

namespace DuckGame
{
    public class MaterialRecolor : Material
    {
        public Vec3 color;

        public MaterialRecolor(Vec3 col)
        {
            spsupport = true;
            color = col;
            _effect = Content.Load<MTEffect>("Shaders/recolor");
        }

        public override void Update()
        {
        }

        public override void Apply()
        {
            _effect.effect.Parameters["fcol"].SetValue((Vector3)color);
            base.Apply();
        }
    }
}
