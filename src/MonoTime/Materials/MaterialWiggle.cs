// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialWiggle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._effect = Content.Load<MTEffect>("Shaders/wiggle");
            this._sprite = t;
        }

        public override void Apply()
        {
            if (DuckGame.Graphics.device.Textures[0] != null)
            {
                Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
                this.SetValue("xpos", this._sprite.x);
                this.SetValue("ypos", this._sprite.y);
            }
            foreach (EffectPass pass in this._effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
