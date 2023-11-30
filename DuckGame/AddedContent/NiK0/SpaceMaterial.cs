using DuckGame;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class MaterialSpace : Material 
    {
        public Tex2D _texture;
        private MTEffect _baseeffect;
        public MaterialSpace()
        {
            spsupport = true;
            _baseeffect = Content.Load<MTEffect>("Shaders/gold");
            _texture = Content.Load<Tex2D>("starfieldloop");
            _effect = _baseeffect;
        }
        public float xAdd;
        public float yAdd;
        public float xS;
        public float yS;
        public float siz;
        public float sizMult;
        public override void Apply()
        {
            if (MonoMain.UpdateLerpState)
            {
                xAdd += xS;
                yAdd += yS;
            }
            if (Graphics.device.Textures[0] != null)
            {
                _effect = _baseeffect;
                SetValue("width", siz);
                SetValue("height", siz * sizMult);
                SetValue("xpos", xAdd);
                SetValue("ypos", yAdd);
            }
            Graphics.device.Textures[1] = (Texture2D)_texture;
            Graphics.device.SamplerStates[1] = SamplerState.PointWrap;
            foreach (EffectPass pass in effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
