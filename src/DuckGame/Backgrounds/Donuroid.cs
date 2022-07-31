// Decompiled with JetBrains decompiler
// Type: DuckGame.Donuroid
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class Donuroid
    {
        private SpriteMap _image;
        private int _frame;
        public Depth _depth;
        private Vec2 _position;
        private float _scale = 1f;
        private float _sin;

        public Donuroid(
          float xpos,
          float ypos,
          SpriteMap image,
          int frame,
          Depth depth,
          float scale)
        {
            this._image = image;
            this._frame = frame;
            this._depth = depth;
            this._scale = scale;
            this._position = new Vec2(xpos, ypos);
            this._sin = Rando.Float(8f);
        }

        public void Draw(Vec2 pos)
        {
            this._image.frame = this._frame;
            this._image.depth = this._depth;
            this._image.xscale = this._image.yscale = this._scale;
            if (_scale == 1.0)
                this._image.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            else
                this._image.color = Color.White * this._scale;
            Graphics.Draw(_image, pos.x + this._position.x, (float)(pos.y + this._position.y + Math.Sin(_sin) * (_scale * 2.0)));
            this._sin += 0.01f;
        }
    }
}
