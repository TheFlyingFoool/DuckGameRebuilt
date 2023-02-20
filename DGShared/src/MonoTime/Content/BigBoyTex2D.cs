// Decompiled with JetBrains decompiler
// Type: DuckGame.BigBoyTex2D
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
