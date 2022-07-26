// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialRainbow
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialRainbow : Material
    {
        public float offset;
        public float offset2;

        public MaterialRainbow() => this._effect = Content.Load<MTEffect>("Shaders/rainbow");

        public override void Apply()
        {
            this._effect.effect.Parameters["offset"].SetValue(this.offset);
            this._effect.effect.Parameters["offset2"].SetValue(this.offset2);
            foreach (EffectPass pass in this._effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
