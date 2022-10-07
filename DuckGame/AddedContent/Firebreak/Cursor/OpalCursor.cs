using DuckGame.AddedContent.Drake.PolyRender;
using System.Windows;

namespace DuckGame
{
    public class OpalCursor : GameCursor
    {
        public static bool DotFollow = false;

        public override string Id => "opal";
        public override void DrawCursor()
        {
            PolyRenderer.Line(new Vec2(Mouse.position.x - 2f, Mouse.position.y), new Vec2(Mouse.position.x + 2f, Mouse.position.y), 0.5f, Color.White);
            PolyRenderer.Line(new Vec2(Mouse.position.x, Mouse.position.y - 2f), new Vec2(Mouse.position.x, Mouse.position.y + 2), 0.5f, Color.White);

            if (DotFollow)
                PolyRenderer.Circle(Mouse.position, 4f, 24, Color.Red);
        }

        public override void LeftPressed(Vec2 position)
        {
            DotFollow = true;
        }

        public override void LeftReleased(Vec2 position)
        {
            DotFollow = false;
        }
    }
}