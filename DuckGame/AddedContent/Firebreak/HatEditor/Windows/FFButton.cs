using System;
using System.Reflection;

namespace DuckGame
{
    public class FFButton : FFBoundary
    {
        // [frames] | inactive |  active  |
        // no hover |    0     |    2     |
        // hover    |    1     |    3     |
        public SpriteMap Icon;
        public Action OnPress;
        public bool Disable;
        public Action<FFButton> ButtonPostAction;
        public Action<FFButton, bool> ButtonToggleAction;
        
        private bool _glow;
        private bool _actionType;

        public FFButton(Rectangle bounds, SpriteMap icon, Action onPress)
            : this(bounds, icon)
        {
            OnPress = onPress;
            _actionType = true;
        }
        
        public FFButton(Rectangle bounds, SpriteMap icon, FieldInfo toggleBooleanField, object target = null)
            : this(bounds, icon)
        {
            if (toggleBooleanField is null || toggleBooleanField.FieldType != typeof(bool))
                throw new ArgumentException("Field type is not bool", nameof(toggleBooleanField));
            
            _glow = (bool)toggleBooleanField.GetValue(target);

            OnPress = () =>
            {
                bool currentValue = (bool)toggleBooleanField.GetValue(target);
                bool newValue = !currentValue;
                
                _glow = newValue;
                toggleBooleanField.SetValue(target, newValue);
            };
        }
        
        protected FFButton(Rectangle bounds, SpriteMap icon)
            : base(bounds, 1f, BorderStyle.Thin)
        {
            Icon = icon;
        }

        public override void Update(bool focus)
        {
            base.Update(focus);
            
            if (!focus || Disable)
                return;

            bool activate = FeatherFashion.InputMode == EditorInput.Mouse
                ? Mouse.left == InputState.Pressed
                : Input.Pressed(Triggers.Select);
            
            if (activate)
            {
                OnPress?.Invoke();
                ButtonPostAction?.Invoke(this);
                if (!_actionType) ButtonToggleAction?.Invoke(this, _glow);
                
                SFX.Play(_actionType ? "highClick" : "click");
            }
        }

        public override void Draw(bool focus)
        {
            base.Draw(focus);

            bool held = false;

            if (focus && !Disable)
            {
                float sin = ((float)Math.Sin(MonoMain.TotalGameTime.TotalMilliseconds / 100) + 1) / 2;
                
                held = FeatherFashion.InputMode == EditorInput.Mouse
                    ? Mouse.left == InputState.Down
                    : Input.Down(Triggers.Select);

                if (!held)
                {
                    Color highlightColor = !_glow
                        ? FeatherFashion.FFColors.Focus
                        : FeatherFashion.FFColors.PrimaryDim;
                    
                    Graphics.DrawRect(Bounds, highlightColor * sin * Alpha, Depth + 2, false, 0.5f);
                }
            }

            int frame = 0;
            
            // hover
            if (focus && !Disable)
            {
                frame += 1;
            }

            // active
            if ((held && _actionType) || _glow)
            {
                frame += 2;
            }
            
            (Color background, Color foreground)[] schemeMap = {
                (FeatherFashion.FFColors.PrimaryDim, FeatherFashion.FFColors.PrimaryHighlight),
                (FeatherFashion.FFColors.PrimaryDim, FeatherFashion.FFColors.Focus),
                (FeatherFashion.FFColors.Focus, FeatherFashion.FFColors.PrimaryDim),
                (FeatherFashion.FFColors.Focus, FeatherFashion.FFColors.PrimaryHighlight),
            };
            
            Icon.alpha = Alpha;
            Icon.depth = Depth + 3;

            (Color backgroundColor, Color foregroundColor) = schemeMap[frame];
            
            Graphics.DrawRect(Bounds, backgroundColor * Alpha, Depth + 1, true);
            Icon.color = foregroundColor;
            Graphics.Draw(Icon, frame, Bounds.x + Bounds.width / 2f - Icon.w / 2f, Bounds.y);
        }
    }
}