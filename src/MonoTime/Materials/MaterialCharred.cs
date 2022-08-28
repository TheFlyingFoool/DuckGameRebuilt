// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialCharred
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialCharred : Material // works with base & Spriteatlas
    {
        public MaterialCharred() => _effect = Content.Load<MTEffect>("Shaders/charred");

        public override void Apply()
        {
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
