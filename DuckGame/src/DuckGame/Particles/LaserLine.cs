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
