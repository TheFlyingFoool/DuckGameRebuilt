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
            base.Apply();
        }
    }
}
