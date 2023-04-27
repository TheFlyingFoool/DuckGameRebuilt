namespace DuckGame
{
    public abstract class GameCursor
    {
        public static GameCursor? CurrentCursor = null;

        protected abstract void DrawCursor();
        protected virtual void LeftPressed(Vec2 position) { }
        protected virtual void LeftReleased(Vec2 position) { }
        protected virtual void RightPressed(Vec2 position) { }
        protected virtual void RightReleased(Vec2 position) { }
        protected virtual void MiddlePressed(Vec2 position) { }
        protected virtual void MiddleReleased(Vec2 position) { }
        
        [DrawingContext(DrawingLayer.Console, CustomID = "CursorDraw")]
        public static void Draw()
        {
            CurrentCursor?.DrawCursor();
        }

        // im too lazy to hook this to an update method, so change this later
        [DrawingContext(CustomID = "CursorUpdate")]
        public static void Update()
        {
            if (Mouse.left == InputState.Pressed)
                CurrentCursor?.LeftPressed(Mouse.positionConsole);
            if (Mouse.left == InputState.Released)
                CurrentCursor?.LeftReleased(Mouse.positionConsole);

            if (Mouse.right == InputState.Pressed)
                CurrentCursor?.RightPressed(Mouse.positionConsole);
            if (Mouse.right == InputState.Released)
                CurrentCursor?.RightReleased(Mouse.positionConsole);

            if (Mouse.middle == InputState.Pressed)
                CurrentCursor?.MiddlePressed(Mouse.positionConsole);
            if (Mouse.middle == InputState.Released)
                CurrentCursor?.MiddleReleased(Mouse.positionConsole);
        }
    }
}