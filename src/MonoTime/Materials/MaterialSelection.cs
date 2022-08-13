// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialSelection
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialSelection : Material
    {
        public float fade = 1f;

        public MaterialSelection() => _effect = Content.Load<MTEffect>("Shaders/selection");

        public override void Apply()
        {
            SetValue("fade", fade);
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
