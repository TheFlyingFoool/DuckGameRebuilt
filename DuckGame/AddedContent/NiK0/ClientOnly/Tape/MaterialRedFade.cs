using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace DuckGame
{
    public class MaterialRedFade : Material
    {
        public MaterialRedFade()
        {
            _effect = Content.Load<MTEffect>("Shaders/fadeRed");
        }
        //public float time;
        public override void Apply()
        {
            //time += 0.017f;
            //SetValue("time", time);
            //Graphics.device.Textures[0] = thing.graphic.texture;
            //Graphics.device.Textures[1] = thing.graphic.texture;
            Graphics.device.SamplerStates[1] = SamplerState.PointClamp;
            base.Apply();
        }
    }
}
