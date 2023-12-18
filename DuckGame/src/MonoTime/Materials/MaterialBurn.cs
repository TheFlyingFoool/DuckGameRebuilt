using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialBurn : Material // unused
    {
        private Tex2D _burnTexture;
        private float _burnVal;

        public float burnVal
        {
            get => _burnVal;
            set => _burnVal = value;
        }

        public MaterialBurn(float burnVal = 0f)
        {
            _effect = Content.Load<MTEffect>("Shaders/burn");
            _burnTexture = Content.Load<Tex2D>("burn");
            _burnVal = burnVal;
        }

        public override void Apply()
        {
            Tex2D texture = (Tex2D)(Graphics.device.Textures[0] as Texture2D);
            Graphics.device.Textures[1] = (Texture2D)_burnTexture;
            SetValue("width", texture.frameWidth / texture.width);
            SetValue("height", texture.frameHeight / texture.height);
            SetValue("burn", _burnVal);
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
