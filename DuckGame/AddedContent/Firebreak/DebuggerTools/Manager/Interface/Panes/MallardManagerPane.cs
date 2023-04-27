using System;

namespace DuckGame.ConsoleInterface.Panes
{
    public abstract class MallardManagerPane
    {
        public abstract bool Borderless { get; }
        public abstract void Update();
        public abstract void DrawRaw(Depth depth, float deltaUnit);
        public abstract void OnFocus();

        public Rectangle Bounds;
        public MallardManagerPane? SwitchToPane = null;
        public bool Active = true;
    }

    public static class MallardManagerPaneExtensions
    {
        public static void Draw(this MallardManagerPane pane, Depth depth, float deltaUnit)
        {
            if (pane.Borderless)
            {
                pane.DrawRaw(depth, deltaUnit);
                return;
            }

            Rectangle border = pane.Bounds;
            Rectangle newBounds = new(pane.Bounds.x + deltaUnit, pane.Bounds.y + deltaUnit, pane.Bounds.width - (deltaUnit * 2), pane.Bounds.height - (deltaUnit * 2));
            Color borderColor = MallardManager.Colors.PrimarySub;

            pane.Bounds = newBounds;
            
            pane.DrawRaw(depth, deltaUnit);
            Graphics.DrawRect(border, borderColor, depth + 0.05f, false, deltaUnit);
            
            pane.Bounds = border;
        }
    }
}