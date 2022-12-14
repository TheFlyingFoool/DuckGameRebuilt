// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialAlbum
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MaterialAlbum : Material //unused
    {
        private Tex2D _albumTexture;

        public MaterialAlbum()
        {
            _effect = Content.Load<MTEffect>("Shaders/album");
            _albumTexture = Content.Load<Tex2D>("playBookPageOffset");
        }

        public override void Apply()
        {
            Graphics.device.Textures[1] = (Texture2D)_albumTexture;
            Graphics.device.SamplerStates[1] = SamplerState.PointClamp;
            foreach (EffectPass pass in _effect.effect.CurrentTechnique.Passes)
                pass.Apply();
        }
    }
}
