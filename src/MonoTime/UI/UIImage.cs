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
            _image = new Sprite(imageVal);
            _collisionSize = new Vec2(_image.w, _image.h);
            _image.CenterOrigin();
            align = al;
        }

        public UIImage(Sprite imageVal, UIAlign al = UIAlign.Left)
          : base(0f, 0f, -1f, -1f)
        {
            _image = imageVal;
            _collisionSize = new Vec2(_image.w, _image.h);
            _image.CenterOrigin();
            align = al;
        }

        public UIImage(Sprite imageVal, UIAlign al, float s = 1f, float yOff = 0f)
          : base(0f, 0f, -1f, -1f)
        {
            _image = imageVal;
            _collisionSize = new Vec2(_image.w * s, _image.h * s);
            _image.CenterOrigin();
            scale = new Vec2(s);
            align = al;
            yOffset = yOff;
        }

        public override void Draw()
        {
            _image.scale = scale;
            _image.alpha = alpha;
            _image.depth = depth;
            Graphics.Draw(_image, x, y + yOffset);
            base.Draw();
        }
    }
}
