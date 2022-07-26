// Decompiled with JetBrains decompiler
// Type: DuckGame.DistanceMarker
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DistanceMarker : Thing
    {
        private BitmapFont _font;
        private Sprite _distanceSign;
        private int _distance;

        public DistanceMarker(float xpos, float ypos, int dist)
          : base(xpos, ypos)
        {
            this._distanceSign = new Sprite("distanceSign");
            this._distanceSign.CenterOrigin();
            this._distance = dist;
            this._font = new BitmapFont("biosFont", 8);
            this._distanceSign.CenterOrigin();
            this.collisionOffset = new Vec2(-8f, -6f);
            this.collisionSize = new Vec2(16f, 13f);
            this.center = new Vec2((float)(this._distanceSign.w / 2), (float)(this._distanceSign.h / 2));
        }

        public override void Draw()
        {
            this._distanceSign.depth = this.depth;
            Graphics.Draw(this._distanceSign, this.x, this.y);
            string text = Change.ToString((object)this._distance);
            this._font.Draw(text, this.x - this._font.GetWidth(text) / 2f, (float)((double)this.y - (double)this._font.height / 2.0 + 1.0), Color.DarkSlateGray, this.depth + 1);
        }
    }
}
