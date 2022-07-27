// Decompiled with JetBrains decompiler
// Type: DuckGame.TileButton
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

        public override bool visible => this._visibleBinding == null ? base.visible : (bool)this._visibleBinding.value;

        public string hoverText => this._hoverText;

        public bool hover
        {
            get => this._hover;
            set => this._hover = value;
        }

        public InputProfile focus
        {
            get => this._focus;
            set => this._focus = value;
        }

        public TileButton(
          float xpos,
          float ypos,
          FieldBinding binding,
          FieldBinding visibleBinding,
          SpriteMap image,
          string hover,
          TileButtonAlign align = TileButtonAlign.None,
          float angleDeg = 0.0f)
          : base(xpos, ypos)
        {
            this._sprite = image;
            this._hoverText = hover;
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
            image.center = new Vec2(image.w / 2, image.h / 2);
            this._binding = binding;
            this._visibleBinding = visibleBinding;
            this._align = align;
            this._alignOffset = new Vec2(xpos, ypos);
            this.angleDegrees = angleDeg;
        }

        public override void Update()
        {
            if (!this.visible)
                this.position = new Vec2(-9999f, -9999f);
            else
                this.position = (this._binding.thing as Editor).GetAlignOffset(this._align) + this._alignOffset;
            bool flag1 = false;
            bool flag2 = false;
            float num = 1f;
            if (Editor.inputMode == EditorInput.Mouse && this._hover)
            {
                if ((double)Mouse.scroll > 0.0)
                    flag2 = true;
                else if ((double)Mouse.scroll < 0.0)
                    flag1 = true;
                if (Mouse.middle == InputState.Down)
                    num = 0.1f;
            }
            if (this._focus == null && (Editor.inputMode != EditorInput.Mouse || !this._hover))
                return;
            if (this._binding.value.GetType() == typeof(float))
            {
                if (((this._focus == null || !this._focus.Pressed("MENULEFT") ? (Keyboard.Pressed(Keys.Left) ? 1 : 0) : 1) | (flag2 ? 1 : 0)) != 0)
                {
                    this._binding.value = Math.Max((float)this._binding.value - this._binding.inc * num, this._binding.min);
                }
                else
                {
                    if (((this._focus == null || !this._focus.Pressed("MENURIGHT") ? (Keyboard.Pressed(Keys.Right) ? 1 : 0) : 1) | (flag1 ? 1 : 0)) == 0)
                        return;
                    this._binding.value = Math.Min((float)this._binding.value + this._binding.inc * num, this._binding.max);
                }
            }
            else if (this._binding.value.GetType() == typeof(int))
            {
                if (((this._focus == null || !this._focus.Pressed("MENULEFT") ? (Keyboard.Pressed(Keys.Left) ? 1 : 0) : 1) | (flag2 ? 1 : 0)) != 0)
                {
                    this._binding.value = (int)Math.Max((int)this._binding.value - this._binding.inc * num, this._binding.min);
                }
                else
                {
                    if (((this._focus == null || !this._focus.Pressed("MENURIGHT") ? (Keyboard.Pressed(Keys.Right) ? 1 : 0) : 1) | (flag1 ? 1 : 0)) == 0)
                        return;
                    this._binding.value = (int)Math.Min((int)this._binding.value + this._binding.inc * num, this._binding.max);
                }
            }
            else if (this._binding.value.GetType() == typeof(Vec2))
            {
                if (((this._focus == null || !this._focus.Pressed("MENULEFT") ? (Keyboard.Pressed(Keys.Left) ? 1 : 0) : 1) | (flag2 ? 1 : 0)) != 0)
                {
                    Vec2 vec2 = (Vec2)this._binding.value;
                    vec2.x = Math.Max(vec2.x - this._binding.inc * num, this._binding.min);
                    this._binding.value = vec2;
                }
                else if (((this._focus == null || !this._focus.Pressed("MENURIGHT") ? (Keyboard.Pressed(Keys.Right) ? 1 : 0) : 1) | (flag1 ? 1 : 0)) != 0)
                {
                    Vec2 vec2 = (Vec2)this._binding.value;
                    vec2.x = Math.Min(vec2.x + this._binding.inc * num, this._binding.max);
                    this._binding.value = vec2;
                }
                else if (((this._focus == null || !this._focus.Pressed("MENUUP") ? (Keyboard.Pressed(Keys.Up) ? 1 : 0) : 1) | (flag2 ? 1 : 0)) != 0)
                {
                    Vec2 vec2 = (Vec2)this._binding.value;
                    vec2.y = Math.Max(vec2.y - this._binding.inc * num, this._binding.min);
                    this._binding.value = vec2;
                }
                else
                {
                    if (((this._focus == null || !this._focus.Pressed("MENUDOWN") ? (Keyboard.Pressed(Keys.Down) ? 1 : 0) : 1) | (flag1 ? 1 : 0)) == 0)
                        return;
                    Vec2 vec2 = (Vec2)this._binding.value;
                    vec2.y = Math.Min(vec2.y + this._binding.inc * num, this._binding.max);
                    this._binding.value = vec2;
                }
            }
            else
            {
                if (!(this._binding.value.GetType() == typeof(bool)) || ((this._focus == null || !this._focus.Pressed("SELECT") ? (Mouse.left == InputState.Pressed ? 1 : 0) : 1) | (flag1 ? 1 : 0) | (flag2 ? 1 : 0)) == 0)
                    return;
                this._binding.value = !(bool)this._binding.value;
            }
        }

        public override void Draw()
        {
            this._sprite.frame = this._hover ? 1 : 0;
            this._sprite.angle = this.angle;
            if (this._binding.value.GetType() == typeof(bool))
            {
                bool flag = (bool)this._binding.value;
                this._sprite.color = Color.White * (flag ? 1f : 0.3f);
            }
            Graphics.Draw(_sprite, this.x, this.y);
            if (this._binding.value.GetType() == typeof(float))
                Graphics.DrawString(((float)this._binding.value).ToString("0.00"), new Vec2(this.x + 12f, this.y - 4f), Color.White);
            if (this._binding.value.GetType() == typeof(int))
                Graphics.DrawString(((int)this._binding.value).ToString(), new Vec2(this.x + 12f, this.y - 4f), Color.White);
            this._hover = false;
            base.Draw();
        }
    }
}
