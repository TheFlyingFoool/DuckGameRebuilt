using System;

namespace DuckGame.ConsoleInterface.Panes
{
    public class MMBorderPane : MallardManagerPane
    {
        public override bool Borderless { get; } = true;
        public MallardManagerPane Child;
        public float BorderThickness;
        public Color BorderColor;

        public MMBorderPane(MallardManagerPane childPane, float borderThickness)
            : this(childPane, borderThickness, MallardManager.Colors.PrimarySub)
        { }

        public MMBorderPane(MallardManagerPane childPane, float borderThickness, Color borderColor)
        {
            Child = childPane;
            BorderThickness = borderThickness;
            BorderColor = borderColor;
        }

        public override void Update()
        {
            Child.Update();
        }

        public override void DrawRaw(Depth depth, float deltaUnit)
        {
            Rectangle innerBounds = new(Bounds.x + deltaUnit * BorderThickness, Bounds.y + deltaUnit * BorderThickness, Bounds.width - (deltaUnit * 2 * BorderThickness), Bounds.height - (deltaUnit * 2 * BorderThickness));
            
            Graphics.DrawRect(Bounds, BorderColor, depth + 0.02f, false, BorderThickness * deltaUnit);

            Child.Bounds = innerBounds;
            Child.DrawRaw(depth, deltaUnit);
        }

        public override void OnFocus()
        {
            Child.OnFocus();
        }
    }
}