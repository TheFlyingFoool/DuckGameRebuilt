namespace DuckGame
{
    public class CapePeice
    {
        internal Vec2 position;
        internal Vec2 p1;
        internal Vec2 p2;
        internal Vec2 scale = new Vec2(1f, 1f);
        internal float wide = 1f;

        internal CapePeice(float _x, float _y, float _width, Vec2 _p1, Vec2 _p2)
        {
            position.x = _x;
            position.y = _y;
            wide = _width;
            p1 = _p1;
            p2 = _p2;
        }
    }
}
