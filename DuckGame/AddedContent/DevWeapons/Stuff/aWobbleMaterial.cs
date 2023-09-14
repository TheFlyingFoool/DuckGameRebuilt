using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class aWobbleMaterial : Material
    {
        public aWobbleMaterial(Thing t, float g, float mult = 0.05f)
        {
            multi = mult;
            _effect = Content.Load<MTEffect>("Shaders/aWobble");
            glow = g;
            _thing = t;
        }
        public override void Apply()
        {
            _time += 0.016f * timeMult;
            if (Graphics.device.Textures[0] != null)
            {
                Tex2D tex2D = Graphics.device.Textures[0] as Texture2D;
                SetValue("width", tex2D.frameWidth / (float)tex2D.width);
                SetValue("height", tex2D.frameHeight / (float)tex2D.height);
                if (_thing != null)
                {
                    SetValue("xpos", _thing.x);
                    SetValue("ypos", _thing.y);
                    SetValue("time", _time);
                    SetValue("glow", glow);
                    SetValue("multi", multi);
                }
            }
            Graphics.device.SamplerStates[1] = SamplerState.PointClamp;
            foreach (EffectPass effectPass in _effect.effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
            }
        }

        public float timeMult = 1;

        private Thing _thing;

        private float _time;

        public float multi;

        public float glow;
    }
}
