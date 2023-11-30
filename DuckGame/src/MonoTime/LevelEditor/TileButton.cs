using System;

namespace DuckGame
{
    public class TileButton : Thing
    {
        private FieldBinding _binding;
        private FieldBinding _visibleBinding;
        private SpriteMap _sprite;
        private string _hoverText;
        private bool _hover;
        private InputProfile _focus;
        private TileButtonAlign _align;
        private Vec2 _alignOffset = Vec2.Zero;

        public override bool visible => _visibleBinding == null ? base.visible : (bool)_visibleBinding.value;

        public string hoverText => _hoverText;

        public bool hover
        {
            get => _hover;
            set => _hover = value;
        }

        public InputProfile focus
        {
            get => _focus;
            set => _focus = value;
        }

        public TileButton(
          float xpos,
          float ypos,
          FieldBinding binding,
          FieldBinding visibleBinding,
          SpriteMap image,
          string hover,
          TileButtonAlign align = TileButtonAlign.None,
          float angleDeg = 0f)
          : base(xpos, ypos)
        {
            _sprite = image;
            _hoverText = hover;
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            image.center = new Vec2(image.w / 2, image.h / 2);
            _binding = binding;
            _visibleBinding = visibleBinding;
            _align = align;
            _alignOffset = new Vec2(xpos, ypos);
            angleDegrees = angleDeg;
        }

        public override void Update()
        {
            if (!visible)
                position = new Vec2(-9999f, -9999f);
            else
                position = (_binding.thing as Editor).GetAlignOffset(_align) + _alignOffset;
            bool flag1 = false;
            bool flag2 = false;
            float num = 1f;
            if (Editor.inputMode == EditorInput.Mouse && _hover)
            {
                if (Mouse.scroll > 0)
                    flag2 = true;
                else if (Mouse.scroll < 0)
                    flag1 = true;
                if (Mouse.middle == InputState.Down)
                    num = 0.1f;
            }
            if (_focus == null && (Editor.inputMode != EditorInput.Mouse || !_hover))
                return;
            if (_binding.value.GetType() == typeof(float))
            {
                if (((_focus == null || !_focus.Pressed(Triggers.MenuLeft) ? (Keyboard.Pressed(Keys.Left) ? 1 : 0) : 1) | (flag2 ? 1 : 0)) != 0)
                {
                    _binding.value = Math.Max((float)_binding.value - _binding.inc * num, _binding.min);
                }
                else
                {
                    if (((_focus == null || !_focus.Pressed(Triggers.MenuRight) ? (Keyboard.Pressed(Keys.Right) ? 1 : 0) : 1) | (flag1 ? 1 : 0)) == 0)
                        return;
                    _binding.value = Math.Min((float)_binding.value + _binding.inc * num, _binding.max);
                }
            }
            else if (_binding.value.GetType() == typeof(int))
            {
                if (((_focus == null || !_focus.Pressed(Triggers.MenuLeft) ? (Keyboard.Pressed(Keys.Left) ? 1 : 0) : 1) | (flag2 ? 1 : 0)) != 0)
                {
                    _binding.value = (int)Math.Max((int)_binding.value - _binding.inc * num, _binding.min);
                }
                else
                {
                    if (((_focus == null || !_focus.Pressed(Triggers.MenuRight) ? (Keyboard.Pressed(Keys.Right) ? 1 : 0) : 1) | (flag1 ? 1 : 0)) == 0)
                        return;
                    _binding.value = (int)Math.Min((int)_binding.value + _binding.inc * num, _binding.max);
                }
            }
            else if (_binding.value.GetType() == typeof(Vec2))
            {
                if (((_focus == null || !_focus.Pressed(Triggers.MenuLeft) ? (Keyboard.Pressed(Keys.Left) ? 1 : 0) : 1) | (flag2 ? 1 : 0)) != 0)
                {
                    Vec2 vec2 = (Vec2)_binding.value;
                    vec2.x = Math.Max(vec2.x - _binding.inc * num, _binding.min);
                    _binding.value = vec2;
                }
                else if (((_focus == null || !_focus.Pressed(Triggers.MenuRight) ? (Keyboard.Pressed(Keys.Right) ? 1 : 0) : 1) | (flag1 ? 1 : 0)) != 0)
                {
                    Vec2 vec2 = (Vec2)_binding.value;
                    vec2.x = Math.Min(vec2.x + _binding.inc * num, _binding.max);
                    _binding.value = vec2;
                }
                else if (((_focus == null || !_focus.Pressed(Triggers.MenuUp) ? (Keyboard.Pressed(Keys.Up) ? 1 : 0) : 1) | (flag2 ? 1 : 0)) != 0)
                {
                    Vec2 vec2 = (Vec2)_binding.value;
                    vec2.y = Math.Max(vec2.y - _binding.inc * num, _binding.min);
                    _binding.value = vec2;
                }
                else
                {
                    if (((_focus == null || !_focus.Pressed(Triggers.MenuDown) ? (Keyboard.Pressed(Keys.Down) ? 1 : 0) : 1) | (flag1 ? 1 : 0)) == 0)
                        return;
                    Vec2 vec2 = (Vec2)_binding.value;
                    vec2.y = Math.Min(vec2.y + _binding.inc * num, _binding.max);
                    _binding.value = vec2;
                }
            }
            else
            {
                if (!(_binding.value.GetType() == typeof(bool)) || ((_focus == null || !_focus.Pressed(Triggers.Select) ? (Mouse.left == InputState.Pressed ? 1 : 0) : 1) | (flag1 ? 1 : 0) | (flag2 ? 1 : 0)) == 0)
                    return;
                _binding.value = !(bool)_binding.value;
            }
        }

        public override void Draw()
        {
            _sprite.frame = _hover ? 1 : 0;
            _sprite.angle = angle;
            if (_binding.value.GetType() == typeof(bool))
            {
                bool flag = (bool)_binding.value;
                _sprite.color = Color.White * (flag ? 1f : 0.3f);
            }
            Graphics.Draw(_sprite, x, y);
            if (_binding.value.GetType() == typeof(float))
                Graphics.DrawString(((float)_binding.value).ToString("0.00"), new Vec2(x + 12f, y - 4f), Color.White);
            if (_binding.value.GetType() == typeof(int))
                Graphics.DrawString(((int)_binding.value).ToString(), new Vec2(x + 12f, y - 4f), Color.White);
            _hover = false;
            base.Draw();
        }
    }
}
