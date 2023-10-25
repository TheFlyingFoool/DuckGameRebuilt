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
            if (alpha < 0)
                Level.Remove(this);
            x += _moveSpeed;
        }

        public override void Draw() => Graphics.DrawLine(position, position + new Vec2(0f, -1200f), _color * alpha, _thickness, (Depth)0.9f);
    }
}
