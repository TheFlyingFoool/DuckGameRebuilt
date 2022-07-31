// Decompiled with JetBrains decompiler
// Type: DuckGame.UIImage
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIImage : UIComponent
    {
        public Sprite _image;
        private float yOffset;

        public UIImage(string imageVal, UIAlign al = UIAlign.Left)
          : base(0f, 0f, -1f, -1f)
        {
            this._image = new Sprite(imageVal);
            this._collisionSize = new Vec2(_image.w, _image.h);
            this._image.CenterOrigin();
            this.align = al;
        }

        public UIImage(Sprite imageVal, UIAlign al = UIAlign.Left)
          : base(0f, 0f, -1f, -1f)
        {
            this._image = imageVal;
            this._collisionSize = new Vec2(_image.w, _image.h);
            this._image.CenterOrigin();
            this.align = al;
        }

        public UIImage(Sprite imageVal, UIAlign al, float s = 1f, float yOff = 0f)
          : base(0f, 0f, -1f, -1f)
        {
            this._image = imageVal;
            this._collisionSize = new Vec2(_image.w * s, _image.h * s);
            this._image.CenterOrigin();
            this.scale = new Vec2(s);
            this.align = al;
            this.yOffset = yOff;
        }

        public override void Draw()
        {
            this._image.scale = this.scale;
            this._image.alpha = this.alpha;
            this._image.depth = this.depth;
            Graphics.Draw(this._image, this.x, this.y + this.yOffset);
            base.Draw();
        }
    }
}
