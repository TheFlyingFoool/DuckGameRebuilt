using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialEnergyChainsaw : Material
    {
        private Tex2D _energyTexture;
        private EnergyChainsaw _thing;
        public float _time;
        public float glow;


        public MaterialEnergyChainsaw(EnergyChainsaw t)
        {
            spsupport = true;
            _effect = Content.Load<MTEffect>("Shaders/energyChainsaw");
            _energyTexture = Content.Load<Tex2D>("energyTex");
            _thing = t;
        }
        public override void Apply()
        {
            if (MonoMain.UpdateLerpState) _time += 0.016f;
            if (Graphics.device.Textures[0] != null)
            {
                Tex2D texture = Graphics.device.Textures[0] as Texture2D;
                SetValue("width", texture.frameWidth / texture.width);
                SetValue("height", texture.frameHeight / texture.height);
                SetValue("xpos", _thing.x);
                SetValue("ypos", _thing.y);
                SetValue("time", _time);
                SetValue("glow", glow);
                SetValue("bladeColor", _thing.bladeColor);
            }
            Graphics.device.Textures[1] = (Texture2D)_energyTexture;
            Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            base.Apply();
        }
    }
}
