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
            get => _selected;
            set => _selected = value;
        }

        public bool isBackButton => _isBackButton;

        public UIMenuAction menuAction
        {
            get => _action;
            set => _action = value;
        }

        public void SetFont(BitmapFont font) => _textElement.SetFont(font);

        public string text
        {
            get
            {
                if (_textElement == null)
                    return "";
                return _textElement.text;
            }
            set
            {
                _textElement.text = value;
                _dirty = _textElement.dirty = true;
            }
        }

        public UIMenuItem(string text, UIMenuAction action = null, UIAlign al = UIAlign.Center, Color c = default(Color), bool backButton = false)
          : base(true, 8, 1f)
        {
            if (c == new Color())
                c = Colors.MenuOption;
            _textElement = new UIText(text, c)
            {
                align = UIAlign.Left
            };
            rightSection.Add(_textElement, true);
            _arrow = new UIImage("contextArrowRight")
            {
                align = UIAlign.Right,
                visible = false
            };
            leftSection.Add(_arrow, true);
            _action = action;
            align = al;
            _isBackButton = backButton;
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
            _textElement = new UIText(pTextFunc, c)
            {
                align = UIAlign.Left
            };
            rightSection.Add(_textElement, true);
            _arrow = new UIImage("contextArrowRight")
            {
                align = UIAlign.Right,
                visible = false
            };
            leftSection.Add(_arrow, true);
            _action = action;
            align = al;
            _isBackButton = backButton;
        }

        public UIMenuItem(UIMenuAction action = null, Color c = default(Color))
          : base(true, 8, 1f)
        {
            _action = action;
        }

        public override void Update()
        {
            _arrow.visible = _selected;
            if (_action != null)
                _action.Update();
            base.Update();
        }

        public virtual void Activate(string trigger)
        {
            if (_action == null || !(trigger == Triggers.Select))
                return;
            _action.Activate();
        }
    }
}
