using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialWiggle : Material
    {
        private Sprite _sprite;

        public MaterialWiggle(Sprite t)
        {
            spsupport = true;
            _effect = Content.Load<MTEffect>("Shaders/wiggle");
            _sprite = t;
        }

        public override void Apply()
        {
            if (Graphics.device.Textures[0] != null)
            {
                Tex2D texture = (Tex2D)(Graphics.device.Textures[0] as Texture2D);
                SetValue("xpos", _sprite.x);
                SetValue("ypos", _sprite.y);
            }
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
