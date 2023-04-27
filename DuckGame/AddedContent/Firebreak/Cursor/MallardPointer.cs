using DuckGame.ConsoleInterface;

namespace DuckGame
{
    public class MallardPointer : GameCursor
    {
        public static Tex2D DefaultTexture;
        public static Tex2D? Texture = null;
        public static Vec2 LastClickPosition = Vec2.MinValue;

        protected override void LeftPressed(Vec2 position) => LastClickPosition = position;
        protected override void RightPressed(Vec2 position) => LeftPressed(position);
        protected override void MiddlePressed(Vec2 position) => LeftPressed(position);

        [PostInitialize]
        public static void Initialize()
        {
            Tex2D texture = new(2, 2);
            texture.Transform((_, _) => (Color) MallardManager.Config.Colors.UserOverlay);
            DefaultTexture = texture;
        }

        protected override void DrawCursor()
        {
            (float x, float y) = Mouse.positionConsole;
            Tex2D texture = Texture ?? DefaultTexture;

            float zoom = MallardManager.Config.Zoom;

            x -= zoom * (texture.width / 2f);
            y -= zoom * (texture.height / 2f);
            
            Graphics.Draw(texture, x, y, zoom, zoom, 2f);
        }
    }
}