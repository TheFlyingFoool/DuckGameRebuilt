using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialGlitch : Material //unused
    {
        private Tex2D _goldTexture;
        private Thing _thing;
        public float amount;
        public float yoffset;
        private int lockframes;

        public MaterialGlitch(Thing t)
        {
            _effect = Content.Load<MTEffect>("Shaders/glitch");
            _goldTexture = Content.Load<Tex2D>("glitchMap3");
            _thing = t;
        }

        public override void Apply()
        {
            if (Graphics.device.Textures[0] != null)
            {
                //Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
                SetValue("width", _thing.graphic.texture.frameWidth / _thing.graphic.texture.width);
                SetValue("height", _thing.graphic.texture.frameHeight / _thing.graphic.texture.height);
                SetValue("frameWidth", _thing.graphic.texture.frameWidth);
                SetValue("amount", amount);
                SetValue("yoff", yoffset);
                SetValue("xpos", _thing.x);
                SetValue("ypos", _thing.y);
            }
            Graphics.device.Textures[1] = (Texture2D)_goldTexture;
            Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            base.Apply();
            if (lockframes > 0)
            {
                --lockframes;
            }
            else
            {
                if (Rando.Float(1f) > 0.85f)
                    lockframes = Rando.Int(2, 12);
                if (Rando.Float(1f) > 0.2f)
                    amount = Lerp.Float(amount, 0f, 0.05f);
                if (Rando.Float(1f) <= 0.98f)
                    return;
                amount += 0.3f;
            }
        }
    }
}
