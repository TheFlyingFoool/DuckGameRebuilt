using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialFunBeam : Material
    {
        private Thing _thing;
        public float intensity;

        public MaterialFunBeam(Thing t)
        {
            spsupport = false;
            _effect = Content.Load<MTEffect>("Shaders/funbeam");
            _thing = t;
        }

        public override void Apply()
        {
            if (Graphics.device.Textures[0] != null)
            {
                //Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
                SetValue("intensity", intensity);
            }
            Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
