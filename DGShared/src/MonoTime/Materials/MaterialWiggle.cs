// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialWiggle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
