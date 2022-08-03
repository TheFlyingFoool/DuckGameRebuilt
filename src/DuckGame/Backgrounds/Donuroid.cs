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
            _image = image;
            _frame = frame;
            _depth = depth;
            _scale = scale;
            _position = new Vec2(xpos, ypos);
            _sin = Rando.Float(8f);
        }

        public void Draw(Vec2 pos)
        {
            _image.frame = _frame;
            _image.depth = _depth;
            _image.xscale = _image.yscale = _scale;
            if (_scale == 1.0)
                _image.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            else
                _image.color = Color.White * _scale;
            Graphics.Draw(_image, pos.x + _position.x, (float)(pos.y + _position.y + Math.Sin(_sin) * (_scale * 2.0)));
            _sin += 0.01f;
        }
    }
}
