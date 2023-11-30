using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialSunshineBare : Material // to test
    {
        public MaterialSunshineBare() => _effect = Content.Load<MTEffect>("Shaders/baresunshine");

        public override void Apply()
        {
            Graphics.device.SamplerStates[0] = SamplerState.LinearClamp;
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
