using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialSunshine : Material // to test
    {
        private RenderTarget2D _colorMap;

        public MaterialSunshine(RenderTarget2D col)
        {
            _effect = Content.Load<MTEffect>("Shaders/sunshine");
            _colorMap = col;
        }

        public override void Apply()
        {
            Graphics.device.Textures[1] = (Texture2D)_colorMap;
            Graphics.device.SamplerStates[1] = SamplerState.PointClamp;
            Graphics.device.SamplerStates[0] = SamplerState.PointClamp;
            base.Apply();
        }
    }
}
