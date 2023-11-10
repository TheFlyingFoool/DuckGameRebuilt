using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class BigBoyTex2D : Tex2D
    {
        public float scaleFactor;

        public override int width => _base == null ? -1 : (int)(_base.Width * scaleFactor);

        public override int height => _base == null ? -1 : (int)(_base.Height * scaleFactor);

        public BigBoyTex2D(Texture2D tex, string texName, short curTexIndex = 0)
          : base(tex, texName, curTexIndex)
        {
        }

        public BigBoyTex2D(int width, int height)
          : base(width, height)
        {
        }
    }
}
