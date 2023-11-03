using Microsoft.Xna.Framework;

namespace DuckGame
{
    public class MaterialPersona : Material
    {
        private DuckPersona persona;

        public MaterialPersona(DuckPersona pPersona)
        {
            spsupport = true;
            persona = pPersona;
            _effect = Content.Load<MTEffect>("Shaders/recolor_duo");
        }

        public override void Update()
        {
        }

        public override void Apply()
        {
            _effect.effect.Parameters["replace1"].SetValue((Vector3)(persona.color / byte.MaxValue));
            _effect.effect.Parameters["replace2"].SetValue((Vector3)(persona.colorDark / byte.MaxValue));
            base.Apply();
        }
    }
}
