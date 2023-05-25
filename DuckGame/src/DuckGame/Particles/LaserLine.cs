// Decompiled with JetBrains decompiler
// Type: DuckGame.LaserLine
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class LaserLine : Thing
    {
        private float _moveSpeed;
        private float _thickness;
        private Color _color;
        private Vec2 _move;
        private Vec2 _target;
        private float fade = 0.06f;

        public LaserLine(
          Vec2 pos,
          Vec2 target,
          Vec2 moveVector,
          float moveSpeed,
          Color color,
          float thickness,
          float f = 0.06f)
          : base(pos.x, pos.y)
        {
            _moveSpeed = moveSpeed;
            _color = color;
            _thickness = thickness;
            _move = moveVector;
            _target = target;
            fade = f;
            shouldbegraphicculled = false;
        }

        public override void Update()
        {
            alpha -= fade;
            if (alpha < 0)
                Level.Remove(this);
            x += _move.x * _moveSpeed;
            y += _move.y * _moveSpeed;
        }

        public override void Draw() => Graphics.DrawLine(position, position + _target, _color * alpha, _thickness, (Depth)0.9f);
    }
}
