using System.Collections.Generic;

namespace DuckGame
{
    public class PortalDoor
    {
        public Vec2 point1;
        public Vec2 point2;
        public Vec2 center;
        public bool horizontal;
        public bool isLeft;
        public Rectangle rect;
        public Layer layer;
        public List<Block> collision = new List<Block>();

        public float top => point1.y;

        public float left => point1.x;

        public float bottom => point2.y;

        public float right => point2.x;

        public void Update()
        {
            Vec2 vec2 = Vec2.Transform(new Vec2(center.x, center.y), Level.current.camera.getMatrix());
            int num1 = (int)vec2.x;
            if (num1 < 0)
                num1 = 0;
            if (num1 > Graphics.width)
                num1 = Graphics.width;
            int num2 = (int)vec2.y;
            if (num2 < 0)
                num2 = 0;
            if (num2 > Graphics.height)
                num2 = Graphics.height;
            if (horizontal)
            {
                if (isLeft)
                    layer.scissor = new Rectangle(0f, num2, Graphics.width, Graphics.height - num2);
                else
                    layer.scissor = new Rectangle(0f, 0f, Graphics.width, num2);
            }
            else if (isLeft)
                layer.scissor = new Rectangle(num1, 0f, Graphics.width - num1, Graphics.height);
            else
                layer.scissor = new Rectangle(0f, 0f, num1, Graphics.height);
        }

        public void Draw() => Graphics.DrawLine(point1, point2, Color.Orange, 2f, (Depth)1f);
    }
}
