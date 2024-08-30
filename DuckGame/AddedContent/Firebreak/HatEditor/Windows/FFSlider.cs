using Microsoft.Xna.Framework.Input;
using System;

namespace DuckGame
{
    public class FFSlider : FFBoundary
    {
        private bool _beingDragged = false;
        private Action<float> _onDrag;

        public ProgressValue Progress;

        private float RawValue
        {
            get => (float) Progress.Value;
            set
            {
                if (RawValue == value)
                    return;

                float speed = Math.Abs(RawValue - value) * 4 / (float)Progress.MaximumValue;
                SFX.Play("highClick", Math.Max(0.25f, speed), Math.Min(2, speed));
                
                Progress.Value = value;
                _onDrag?.Invoke(value);
            }
        }
        
        public bool Disable;
        
        public FFSlider(Rectangle bounds, ProgressValue progress, Action<float> onUse)
            : base(bounds, 1f, BorderStyle.Thin)
        {
            Progress = progress;
            _onDrag = onUse;
        }

        public override void Update(bool focus)
        {
            base.Update(focus);
            
            if ((FeatherFashion.InputMode == EditorInput.Mouse
                    && Mouse.left == InputState.Released) ||
                (FeatherFashion.InputMode == EditorInput.Gamepad
                    && Input.Released(Triggers.Select))
                )
            {
                _beingDragged = false;
                return;
            }
            
            if (_beingDragged)
            {
                if (FeatherFashion.InputMode == EditorInput.Gamepad)
                {
                    if (Input.Pressed(Triggers.Left))
                    {
                        RawValue -= (float) Progress.IncrementSize;
                    }
                    else if (Input.Pressed(Triggers.Right))
                    {
                        RawValue += (float) Progress.IncrementSize;
                    }
                }
                else // mouse
                {
                    float mouseXToBounds = Maths.Clamp(Mouse.xScreen - Bounds.x, 0, Bounds.width);
                    float xCompletionPercentage = mouseXToBounds / Bounds.width;

                    RawValue = (float) (Math.Round((xCompletionPercentage * Progress.MaximumValue) / Progress.IncrementSize) * Progress.IncrementSize);
                }
            }

            if (!focus || Disable)
                return;

            if ((FeatherFashion.InputMode == EditorInput.Mouse
                    && Mouse.left == InputState.Pressed) ||
                (FeatherFashion.InputMode == EditorInput.Gamepad
                    && Input.Pressed(Triggers.Select))
                )
            {
                _beingDragged = true;
            }
        }

        public override void Draw(bool focus)
        {
            base.Draw(focus);
            
            if (focus && !Disable)
            {
                float sin = ((float)Math.Sin(MonoMain.TotalGameTime.TotalMilliseconds / 100) + 1) / 2;
                
                bool held = FeatherFashion.InputMode == EditorInput.Mouse
                    ? Mouse.left == InputState.Down
                    : Input.Down(Triggers.Select);

                if (!held)
                {
                    Graphics.DrawRect(Bounds, FeatherFashion.FFColors.Focus * sin * Alpha, Depth + 2, false, 0.5f);
                }
            }

            Color sliderColor = _beingDragged
                ? FeatherFashion.FFColors.Focus
                : FeatherFashion.FFColors.PrimaryHighlight;

            float sliderXPos = (float) Progress.NormalizedValue * (Bounds.Right - Bounds.Left) + Bounds.x;
            Rectangle sliderBarRect = new(sliderXPos - 0.5f, Bounds.y, 1f, Bounds.height);
            Graphics.DrawRect(Bounds, FeatherFashion.FFColors.PrimaryDim * Alpha, Depth + 1);
            Graphics.DrawRect(sliderBarRect, sliderColor * Alpha, Depth + 5);
        }
    }
}