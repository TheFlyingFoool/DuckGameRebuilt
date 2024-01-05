using System;

namespace DuckGame
{
    public class TestLev : Level
    {
        public Vec2 ls = Vec2.Zero;
        public Vec2 le = Vec2.Zero;

        public override void Initialize()
        {
            
        }

        public override void Draw()
        {
            float gridSize = 16;
            int xc = (int) Math.Round(camera.width / gridSize, MidpointRounding.AwayFromZero);
            int yc = (int) Math.Round(camera.height / gridSize, MidpointRounding.AwayFromZero);

            for (int y = 0; y < yc; y++)
            {
                for (int x = 0; x < xc; x++)
                {
                    Rectangle rect = new(x * gridSize, y * gridSize, gridSize, gridSize);
                    Rectangle rectO = new(x * gridSize, y * gridSize, gridSize, gridSize);
                    Color color = Collision.Line(ls, le, rect) ? Color.Lime : Color.Red;
                    
                    Graphics.DrawRect(rect, color, depth: 1f);
                    Graphics.DrawRect(rectO, Color.Yellow, 1.2f, false, gridSize / 64f);
                }
            }
            
            Graphics.DrawRect(Mouse.positionScreen - Vec2.One, Mouse.positionScreen + Vec2.One, Color.White, 2f);
            
            Graphics.DrawRect(ls - Vec2.One, ls + Vec2.One, Color.Black, 1.5f);
            Graphics.DrawRect(le - Vec2.One, le + Vec2.One, Color.Black, 1.5f);
            Graphics.DrawLine(ls, le, Color.Black, 0.5f, 1.5f);
            
            base.Draw();
        }

        public override void Update()
        {
            if (Mouse.left == InputState.Down)
                ls = Mouse.positionScreen;
            
            if (Mouse.right == InputState.Down)
                le = Mouse.positionScreen;
            
            base.Update();
        }
    }
}