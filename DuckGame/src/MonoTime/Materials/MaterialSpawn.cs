// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialSpawn
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialSpawn : Material // unused
    {
        public MaterialSpawn() => _effect = Content.Load<MTEffect>("Shaders/wireframeTex");

        public override void Apply()
        {
            if (DuckGame.Graphics.device.Textures[0] != null)
            {
                Tex2D texture = (Tex2D)(DuckGame.Graphics.device.Textures[0] as Texture2D);
            }
            effect.effect.Parameters["screenCross"].SetValue(0.5f);
            effect.effect.Parameters["scanMul"].SetValue(1f);
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
