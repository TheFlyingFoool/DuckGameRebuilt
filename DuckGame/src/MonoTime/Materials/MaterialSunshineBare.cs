// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialSunshineBare
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialSunshineBare : Material // to test
    {
        public MaterialSunshineBare() => _effect = Content.Load<MTEffect>("Shaders/baresunshine");

        public override void Apply()
        {
            DuckGame.Graphics.device.SamplerStates[0] = SamplerState.LinearClamp;
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
