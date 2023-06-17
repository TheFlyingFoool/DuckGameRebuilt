using System;

namespace DuckGame.ConsoleInterface.Panes
{
    public partial class MMConfigPane
    {
        public static partial class UI
        {
            public class Checkbox : Element
            {
                public bool State
                {
                    get => _stateGetter();
                    set => _stateSetter(value);
                }

                private Func<bool> _stateGetter;
                private Action<bool> _stateSetter;

                private Rectangle CheckboxBounds => new(
                    FrameBounds.x + FrameBounds.width - FrameBounds.height + 1.5f * DeltaUnit,
                    FrameBounds.Top + 1.5f * DeltaUnit,
                    FrameBounds.height - DeltaUnit - DeltaUnit * 2,
                    FrameBounds.height - DeltaUnit - DeltaUnit * 2);

                public Checkbox(Func<bool> stateGetter, Action<bool> stateSetter)
                {
                    _stateGetter = stateGetter;
                    _stateSetter = stateSetter;
                }
                
                public override float Draw(Depth depth, float xOffset)
                {
                    Rectangle checkboxBoundsOffset = CheckboxBounds;
                    checkboxBoundsOffset.x -= xOffset;
                    
                    Graphics.DrawOutlinedRect(checkboxBoundsOffset,
                        State ? MallardManager.Colors.UserOverlay : MallardManager.Colors.Secondary,
                        MallardManager.Colors.SecondarySub, depth + 0.2f, DeltaUnit);

                    return checkboxBoundsOffset.width;
                }

                public override float Update(float xOffset)
                {
                    Rectangle checkboxBoundsOffset = CheckboxBounds;
                    checkboxBoundsOffset.x -= xOffset;
                    
                    if (Mouse.left == InputState.Pressed && checkboxBoundsOffset.Contains(MousePosition))
                        State ^= true;

                    return checkboxBoundsOffset.width;
                }
            }
        }
    }
}