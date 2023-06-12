namespace DuckGame.ConsoleInterface.Panes
{
    public partial class MMConfigPane
    {
        public static partial class UI
        {
            public abstract class Element
            {
                public Rectangle FrameBounds;
                protected static float DeltaUnit => MallardManager.Config.Zoom;
                protected static Vec2 MousePosition => Mouse.positionConsole;

                // protected Element(Rectangle frameBounds)
                // {
                //     FrameBounds = frameBounds;
                // }
                
                public abstract float Draw(Depth depth, float xOffset);
                public abstract float Update(float xOffset);
            }
        }
    }
}