using System;
using System.Drawing;

namespace DuckGame.ConsoleInterface.Panes
{
    public partial class MMConfigPane
    {
        public static partial class UI
        {
            public class Button : Element
            {
                public Action OnClick;

                private Rectangle ButtonBounds => new(
                    FrameBounds.x + FrameBounds.width - FrameBounds.height * 2 + 1.5f * DeltaUnit,
                    FrameBounds.Top + 1.5f * DeltaUnit,
                    FrameBounds.height * 2 - DeltaUnit - DeltaUnit * 2,
                    FrameBounds.height - DeltaUnit - DeltaUnit * 2
                );

                public Button(Action onClick)
                {
                    OnClick = onClick;
                }
                
                public override float Draw(Depth depth, float xOffset)
                {
                    Rectangle buttonBoundsOffset = ButtonBounds;
                    buttonBoundsOffset.x -= xOffset;

                    bool mouseHovered = buttonBoundsOffset.Contains(MousePosition);
                    Graphics.DrawOutlinedRect(buttonBoundsOffset,
                        mouseHovered ? MallardManager.Colors.UserOverlay : MallardManager.Colors.Secondary,
                        MallardManager.Colors.SecondarySub, depth + 0.2f, DeltaUnit);

                    return buttonBoundsOffset.width;
                }

                public override float Update(float xOffset)
                {
                    Rectangle buttonBoundsOffset = ButtonBounds;
                    buttonBoundsOffset.x -= xOffset;
                    
                    if (Mouse.left == InputState.Pressed && buttonBoundsOffset.Contains(MousePosition))
                        OnClick?.Invoke();

                    return buttonBoundsOffset.width;
                }
            }
        }
    }
}