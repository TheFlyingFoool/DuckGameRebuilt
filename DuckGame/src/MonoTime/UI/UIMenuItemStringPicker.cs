// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuItemStringPicker
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class UIMenuItemStringPicker : UIMenuItem
    {
        public bool needsApply;
        public string currentValue;
        public Action<string> selectAction;
        protected FieldBinding _field;
        protected UIText _textItem;
        protected List<string> _valueStrings;
        protected int currentIndex;
        protected bool _useBaseActivationLogic;

        public UIMenuItemStringPicker(
          string text,
          List<string> valStrings,
          FieldBinding field,
          UIMenuAction action = null,
          Color c = default(Color))
          : base(action)
        {
            if (c == new Color())
                c = Colors.MenuOption;
            _valueStrings = valStrings;
            UIDivider component1 = new UIDivider(true, _valueStrings != null ? 0f : 0.8f);
            UIText component2 = new UIText(text, c)
            {
                align = UIAlign.Left
            };
            component1.leftSection.Add(component2, true);
            currentIndex = _valueStrings.IndexOf(field.value as string);
            if (currentIndex < 0)
                currentIndex = 0;
            _textItem = new UIChangingText(-1f, -1f, field, null);
            string str = "";
            foreach (string valString in valStrings)
            {
                if (valString.Length > str.Length)
                    str = valString;
            }
          (_textItem as UIChangingText).defaultSizeString = str;
            _textItem.minLength = str.Length;
            _textItem.text = _valueStrings[currentIndex];
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
            currentValue = _field.value as string;
            controlString = "@WASD@ADJUST @SELECT@APPLY";
        }

        public override void Activate(string trigger)
        {
            if (_useBaseActivationLogic)
                base.Activate(trigger);
            else if (trigger == Triggers.Select)
            {
                if (selectAction != null)
                    selectAction(currentValue);
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
                if (currentIndex > _valueStrings.Count - 1)
                    currentIndex = _valueStrings.Count - 1;
                currentValue = _valueStrings[currentIndex];
                int currentIndex2 = currentIndex;
                if (currentIndex1 != currentIndex2)
                {
                    SFX.DontSave = 1;
                    SFX.Play("textLetter", 0.7f);
                }
                if (_textItem == null)
                    return;
                _textItem.text = _valueStrings[currentIndex];
            }
        }

        public override void Draw()
        {
            if (!selected)
                currentValue = _field.value as string;
            currentIndex = _valueStrings.IndexOf(currentValue);
            if (currentIndex < 0)
                currentIndex = 0;
            if (!_textItem.text.Contains(_valueStrings[currentIndex]))
                _textItem.text = _valueStrings[currentIndex];
            base.Draw();
        }
    }
}
