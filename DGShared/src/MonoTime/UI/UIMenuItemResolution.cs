// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuItemResolution
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class UIMenuItemResolution : UIMenuItem
    {
        public Resolution currentValue;
        public Action selectAction;
        protected FieldBinding _field;
        protected UIText _textItem;
        protected List<Resolution> _values;
        protected int currentIndex;
        private bool showAll;
        private Resolution _current;

        public UIMenuItemResolution(string text, FieldBinding field, UIMenuAction action = null, Color c = default(Color))
          : base(action)
        {
            if (c == new Color())
                c = Colors.MenuOption;
            if (MonoMain._fullScreen)
                controlString = "@WASD@ADJUST @SELECT@APPLY";
            else
                controlString = "@WASD@ADJUST @SELECT@APPLY @MENU2@ALL";
            UIDivider component1 = new UIDivider(true, 0f);
            UIText component2 = new UIText(text, c)
            {
                align = UIAlign.Left
            };
            component1.leftSection.Add(component2, true);
            RefreshValueList();
            currentIndex = _values.IndexOf(field.value as Resolution);
            if (currentIndex < 0)
                currentIndex = 0;
            _textItem = new UIChangingText(-1f, -1f, field, null);
            string str = "";
            foreach (Resolution resolution in _values)
            {
                if (resolution.ToShortString().Length > str.Length)
                    str = resolution.ToShortString();
            }
          (_textItem as UIChangingText).defaultSizeString = str + "   ";
            _textItem.minLength = str.Length + 3;
            _textItem.text = currentIndex > 0 ? _values[currentIndex].ToShortString() + "   " : "None    ";
            _textItem.align = UIAlign.Right;
            component1.rightSection.Add(_textItem, true);
            rightSection.Add(component1, true);
            _arrow = new UIImage("contextArrowRight")
            {
                align = UIAlign.Right,
                visible = false
            };
            leftSection.Add(_arrow, true);
            _field = field;
            currentValue = _field.value as Resolution;
        }

        private void RefreshValueList()
        {
            if (showAll)
            {
                controlString = "@WASD@ADJUST @SELECT@APPLY @MENU2@BASIC";
                _values = Resolution.supportedDisplaySizes[Resolution.current.mode];
            }
            else
            {
                controlString = "@WASD@ADJUST @SELECT@APPLY @MENU2@ALL";
                if (Resolution.current.mode == ScreenMode.Windowed)
                    _values = Resolution.supportedDisplaySizes[Resolution.current.mode].Where(x => x.recommended || x == Resolution.current).ToList();
                else
                    _values = Resolution.supportedDisplaySizes[Resolution.current.mode].Where(x => Math.Abs(x.aspect - Resolution.adapterResolution.aspect) < 0.05f || x == Resolution.current).ToList();
            }
        }

        public override void Activate(string trigger)
        {
            if (trigger == Triggers.Select)
            {
                _field.value = currentValue;
                if (selectAction == null)
                    return;
                selectAction();
            }
            else if (trigger == Triggers.Menu2)
            {
                showAll = !showAll;
                RefreshValueList();
                SFX.Play("textLetter", 0.7f);
                currentIndex = _values.IndexOf(_field.value as Resolution);
                if (currentIndex < 0)
                    currentIndex = 0;
                _textItem.text = _values[currentIndex].ToShortString();
            }
            else if (trigger == Triggers.Select)
            {
                if (selectAction != null)
                    selectAction();
                _field.value = currentValue;
            }
            else
            {
                int currentIndex1 = currentIndex;
                if (trigger == Triggers.MenuLeft)
                    --currentIndex;
                else if (trigger == Triggers.MenuRight)
                    ++currentIndex;
                if (currentIndex < 0)
                    currentIndex = 0;
                if (currentIndex > _values.Count - 1)
                    currentIndex = _values.Count - 1;
                currentValue = _values[currentIndex];
                int currentIndex2 = currentIndex;
                if (currentIndex1 != currentIndex2)
                    SFX.Play("textLetter", 0.7f);
                if (_textItem == null)
                    return;
                _textItem.text = _values[currentIndex].ToShortString();
            }
        }

        public override void Draw()
        {
            if (Resolution.current != _current)
            {
                RefreshValueList();
                _current = Resolution.current;
            }
            if (!selected)
                currentValue = _field.value as Resolution;
            currentIndex = _values.IndexOf(currentValue);
            if (currentIndex < 0)
                currentIndex = _values.Count - 1;
            _textItem.text = _values[currentIndex].ToShortString();
            base.Draw();
        }
    }
}
