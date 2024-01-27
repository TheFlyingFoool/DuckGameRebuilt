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
            if (Level.current == null || Level.current.camera == null || layer == null)
                return;
            Vec2 pos = Vec2.Transform(new Vec2(center.x, center.y), Level.current.camera.getMatrix());
            int xScissor = (int)pos.x;
            if (xScissor < 0)
                xScissor = 0;
            if (xScissor > Graphics.width)
                xScissor = Graphics.width;
            int yScissor = (int)pos.y;
            if (yScissor < 0)
                yScissor = 0;
            if (yScissor > Graphics.height)
                yScissor = Graphics.height;
            if (horizontal)
            {
                if (isLeft)
                    layer.scissor = new Rectangle(0f, yScissor, Graphics.width, Graphics.height - yScissor);
                else
                    layer.scissor = new Rectangle(0f, 0f, Graphics.width, yScissor);
            }
            else if (isLeft)
                layer.scissor = new Rectangle(xScissor, 0f, Graphics.width - xScissor, Graphics.height);
            else
                layer.scissor = new Rectangle(0f, 0f, xScissor, Graphics.height);
        }

        public void Draw() => Graphics.DrawLine(point1, point2, Color.Orange, 2f, (Depth)1f);
    }
}
