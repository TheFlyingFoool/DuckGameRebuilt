﻿using System.Collections.Generic;

namespace DuckGame
{
    public class LUIMenuItemToggle : UIMenuItem
    {
        private FieldBinding _field;
        private FieldBinding _filterBinding;
        private bool _compressed;
        private List<string> _multiToggle;
        private LUIMultiToggle _multiToggleElement;

        public void SetFieldBinding(FieldBinding f)
        {
            _field = f;
            if (_multiToggleElement == null)
                return;
            _multiToggleElement.SetFieldBinding(f);
        }

        public LUIMenuItemToggle(
          string text,
          UIMenuAction action = null,
          FieldBinding field = null,
          Color c = default(Color),
          FieldBinding filterBinding = null,
          List<string> multi = null,
          bool compressedMulti = false,
          bool tiny = false)
          : base(action)
        {
            if (c == new Color())
                c = Colors.MenuOption;
            BitmapFont f = null;
            if (tiny)
                f = new BitmapFont("smallBiosFontUI", 7, 5);
            UIDivider splitter = new UIDivider(true, 0f);
            if (text != "")
            {
                LUIText t = new LUIText(text, c);
                if (tiny)
                    t.SetFont(f);
                t.align = UIAlign.Left;
                splitter.leftSection.Add(t, true);
            }
            if (multi != null)
            {
                _multiToggleElement = new LUIMultiToggle(-1f, -1f, field, multi, compressedMulti)
                {
                    align = compressedMulti ? UIAlign.Right : UIAlign.Right
                };
                if (text != "")
                {
                    splitter.rightSection.Add(_multiToggleElement, true);
                }
                else
                {
                    splitter.leftSection.Add(_multiToggleElement, true);
                    _multiToggleElement.align = UIAlign.Left;
                }
                if (tiny)
                    _multiToggleElement.SetFont(f);
                _multiToggle = multi;
                _compressed = compressedMulti;
            }
            else
            {
                LUIOnOff onOff = new LUIOnOff(-1f, -1f, field, filterBinding);
                if (tiny)
                    onOff.SetFont(f);
                onOff.align = UIAlign.Right;
                splitter.rightSection.Add(onOff, true);
            }
            rightSection.Add(splitter, true);
            if (tiny)
                _arrow = new UIImage("littleContextArrowRight");
            else
                _arrow = new UIImage("contextArrowRight");
            _arrow.align = UIAlign.Right;
            _arrow.visible = false;
            leftSection.Add(_arrow, true);
            _field = field;
            _filterBinding = filterBinding;
            controlString = "@CANCEL@BACK @WASD@ADJUST";
        }

        public override void Activate(string trigger)
        {
            int num1 = 1;
            int num2 = _filterBinding != null ? -1 : 0;
            int num3;
            if (_multiToggle != null)
            {
                num1 = _multiToggle.Count - 1;
                num3 = (int)_field.value;
            }
            else
                num3 = (bool)_field.value ? 1 : 0;
            if (_filterBinding != null && !(bool)_filterBinding.value)
                num3 = -1;
            bool flag = false;
            if (trigger == Triggers.Select || trigger == Triggers.MenuRight)
            {
                ++num3;
                flag = true;
            }
            else if (trigger == Triggers.MenuLeft)
            {
                --num3;
                flag = true;
            }
            if (num3 < num2)
                num3 = num1;
            else if (num3 > num1)
                num3 = num2;
            if (num3 == -1)
            {
                _filterBinding.value = false;
            }
            else
            {
                if (_filterBinding != null)
                    _filterBinding.value = true;
                _field.value = _multiToggle == null ? num3 != 0 : num3;
            }
            if (!flag)
                return;
            SFX.Play("textLetter", 0.7f);
            if (_action == null)
                return;
            _action.Activate();
        }
    }
}