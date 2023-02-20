namespace DuckGame
{
    public abstract class GameCursor
    {
        public abstract string Id { get; }
        public abstract void DrawCursor();

        public virtual void LeftPressed(Vec2 position) { }
        public virtual void LeftReleased(Vec2 position) { }

        public virtual void RightPressed(Vec2 position) { }
        public virtual void RightReleased(Vec2 position) { }

        public virtual void MiddlePressed(Vec2 position) { }
        public virtual void MiddleReleased(Vec2 position) { }
    }
}