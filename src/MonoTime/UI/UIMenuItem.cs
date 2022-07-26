// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class UIMenuItem : UIDivider
    {
        public string controlString;
        private bool _selected;
        protected UIImage _arrow;
        protected UIMenuAction _action;
        protected bool _isBackButton;
        protected UIText _textElement;

        public bool selected
        {
            get => this._selected;
            set => this._selected = value;
        }

        public bool isBackButton => this._isBackButton;

        public UIMenuAction menuAction
        {
            get => this._action;
            set => this._action = value;
        }

        public void SetFont(BitmapFont font) => this._textElement.SetFont(font);

        public string text
        {
            get => this._textElement.text;
            set
            {
                this._textElement.text = value;
                this._dirty = this._textElement.dirty = true;
            }
        }

        public UIMenuItem(string text, UIMenuAction action = null, UIAlign al = UIAlign.Center, Color c = default(Color), bool backButton = false)
          : base(true, 8, 1f)
        {
            if (c == new Color())
                c = Colors.MenuOption;
            this._textElement = new UIText(text, c);
            this._textElement.align = UIAlign.Left;
            this.rightSection.Add((UIComponent)this._textElement, true);
            this._arrow = new UIImage("contextArrowRight");
            this._arrow.align = UIAlign.Right;
            this._arrow.visible = false;
            this.leftSection.Add((UIComponent)this._arrow, true);
            this._action = action;
            this.align = al;
            this._isBackButton = backButton;
        }

        public UIMenuItem(
          Func<string> pTextFunc,
          UIMenuAction action = null,
          UIAlign al = UIAlign.Center,
          Color c = default(Color),
          bool backButton = false)
          : base(true, 8, 1f)
        {
            if (c == new Color())
                c = Colors.MenuOption;
            this._textElement = new UIText(pTextFunc, c);
            this._textElement.align = UIAlign.Left;
            this.rightSection.Add((UIComponent)this._textElement, true);
            this._arrow = new UIImage("contextArrowRight");
            this._arrow.align = UIAlign.Right;
            this._arrow.visible = false;
            this.leftSection.Add((UIComponent)this._arrow, true);
            this._action = action;
            this.align = al;
            this._isBackButton = backButton;
        }

        public UIMenuItem(UIMenuAction action = null, Color c = default(Color))
          : base(true, 8, 1f)
        {
            this._action = action;
        }

        public override void Update()
        {
            this._arrow.visible = this._selected;
            if (this._action != null)
                this._action.Update();
            base.Update();
        }

        public virtual void Activate(string trigger)
        {
            if (this._action == null || !(trigger == "SELECT"))
                return;
            this._action.Activate();
        }
    }
}
