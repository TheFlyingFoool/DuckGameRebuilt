using DuckGame.AddedContent.Drake.PolyRender;

namespace DuckGame
{
    public class OpalCursor : GameCursor
    {
        public static bool DotFollow = false;

        public override string Id => "opal";
        public override void DrawCursor()
        {
            PolyRenderer.Line(Mouse.position.ButX(-2, true), Mouse.position.ButX(2, true), 0.5f, Color.White);
            PolyRenderer.Line(Mouse.position.ButY(-2, true), Mouse.position.ButY(2, true), 0.5f, Color.White);

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