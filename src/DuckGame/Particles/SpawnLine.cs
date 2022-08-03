// Decompiled with JetBrains decompiler
// Type: DuckGame.SpawnLine
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class SpawnLine : Thing
    {
        private float _moveSpeed;
        private float _thickness;
        private Color _color;

        public SpawnLine(
          float xpos,
          float ypos,
          int dir,
          float moveSpeed,
          Color color,
          float thickness)
          : base(xpos, ypos)
        {
            _moveSpeed = moveSpeed;
            _color = color;
            _thickness = thickness;
            offDir = (sbyte)dir;
            layer = Layer.Foreground;
            depth = (Depth)0.9f;
        }

        public override void Update()
        {
            alpha -= 0.03f;
            if (alpha < 0.0)
                Level.Remove(this);
            x += _moveSpeed;
        }

        public override void Draw() => Graphics.DrawLine(position, position + new Vec2(0f, -1200f), _color * alpha, _thickness, (Depth)0.9f);
    }
}
