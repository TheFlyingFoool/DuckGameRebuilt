// Decompiled with JetBrains decompiler
// Type: DuckGame.PortalDoor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        public float top => this.point1.y;

        public float left => this.point1.x;

        public float bottom => this.point2.y;

        public float right => this.point2.x;

        public void Update()
        {
            Vec2 vec2 = Vec2.Transform(new Vec2(this.center.x, this.center.y), Level.current.camera.getMatrix());
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
            if (this.horizontal)
            {
                if (this.isLeft)
                    this.layer.scissor = new Rectangle(0.0f, (float)num2, (float)Graphics.width, (float)(Graphics.height - num2));
                else
                    this.layer.scissor = new Rectangle(0.0f, 0.0f, (float)Graphics.width, (float)num2);
            }
            else if (this.isLeft)
                this.layer.scissor = new Rectangle((float)num1, 0.0f, (float)(Graphics.width - num1), (float)Graphics.height);
            else
                this.layer.scissor = new Rectangle(0.0f, 0.0f, (float)num1, (float)Graphics.height);
        }

        public void Draw() => Graphics.DrawLine(this.point1, this.point2, Color.Orange, 2f, (Depth)1f);
    }
}
