using System.Collections.Generic;

namespace DuckGame
{
    public class LUIMenuItemNumber : UIMenuItem
    {
        protected FieldBinding _field;
        protected int _step;
        protected FieldBinding _upperBoundField;
        protected FieldBinding _lowerBoundField;
        protected FieldBinding _filterField;
        protected LUIText _textItem;
        public List<FieldBinding> percentageGroup = new List<FieldBinding>();
        private List<string> _valueStrings;
        private MatchSetting _setting;
        protected bool _useBaseActivationLogic;

        private bool altKeyDown => Keyboard.Down(Keys.LeftAlt) || Keyboard.Down(Keys.RightAlt);

        public LUIMenuItemNumber(
          string text,
          UIMenuAction action = null,
          FieldBinding field = null,
          int step = 1,
          Color c = default(Color),
          FieldBinding upperBoundField = null,
          FieldBinding lowerBoundField = null,
          string append = "",
          FieldBinding filterField = null,
          List<string> valStrings = null,
          MatchSetting setting = null)
          : base(action)
        {
            _setting = setting;
            if (c == new Color())
                c = Colors.MenuOption;
            _valueStrings = valStrings;
            UIDivider splitter = new UIDivider(true, _valueStrings != null ? 0f : 0.8f);
            LUIText t = new LUIText(text, c)
            {
                align = UIAlign.Left
            };
            splitter.leftSection.Add(t, true);
            if (field == null)
            {
                _textItem = new LUIChangingText(-1f, -1f, field, null)
                {
                    align = UIAlign.Right
                };
                splitter.rightSection.Add(_textItem, true);
            }
            else if (_valueStrings != null)
            {
                if (text == "" || text == null)
                {
                    splitter.leftSection.align = UIAlign.Left;
                    _textItem = t;
                    int index = (int)field.value;
                    if (index >= 0 && index < _valueStrings.Count)
                        _textItem.text = _valueStrings[index];
                }
                else
                {
                    _textItem = new LUIChangingText(-1f, -1f, field, null);
                    int index = (int)field.value;
                    if (index >= 0 && index < _valueStrings.Count)
                        _textItem.text = _valueStrings[index];
                    _textItem.align = UIAlign.Right;
                    splitter.rightSection.Add(_textItem, true);
                }
            }
            else
            {
                LUINumber number = new LUINumber(-1f, -1f, field, append, filterField, _setting)
                {
                    align = UIAlign.Right
                };
                splitter.rightSection.Add(number, true);
            }
            if (_valueStrings != null)
            {
                string str = "";
                foreach (string valueString in _valueStrings)
                {
                    if (valueString.Length > str.Length)
                        str = valueString;
                }
                (_textItem as LUIChangingText).defaultSizeString = str + "   ";
                _textItem.minLength = str.Length + 3;
                _textItem.text = _textItem.text;
            }
            rightSection.Add(splitter, true);
            _arrow = new UIImage("contextArrowRight")
            {
                align = UIAlign.Right,
                visible = false
            };
            leftSection.Add(_arrow, true);
            _field = field;
            _step = step;
            _upperBoundField = upperBoundField;
            _lowerBoundField = lowerBoundField;
            _filterField = filterField;
            controlString = "@CANCEL@BACK @WASD@ADJUST";
        }

        private int GetStep(int current, bool up)
        {
            if (_setting == null || _setting.stepMap == null)
                return _step;
            if (altKeyDown && _setting.altStep != null)
                return _setting.altStep;
            int step = 0;
            foreach (KeyValuePair<int, int> pair in _setting.stepMap)
            {
                step = pair.Value;
                if (up)
                {
                    if (pair.Key > current)
                        break;
                }
                if (!up)
                {
                    if (pair.Key >= current)
                        break;
                }
            }
            return step;
        }

        public override void Activate(string trigger)
        {
            if (_useBaseActivationLogic)
            {
                base.Activate(trigger);
            }
            else
            {
                if (_filterField != null)
                {
                    if (!(bool)_filterField.value && (trigger == Triggers.MenuRight || trigger == Triggers.Select))
                    {
                        SFX.Play("textLetter", 0.7f);
                        _filterField.value = true;
                        _field.value = (int)_field.min;
                        return;
                    }
                    if (!(bool)_filterField.value && trigger == Triggers.MenuLeft)
                    {
                        SFX.Play("textLetter", 0.7f);
                        _filterField.value = true;
                        _field.value = (int)_field.max;
                        return;
                    }
                    if ((bool)_filterField.value && trigger == Triggers.MenuLeft && (int)_field.value == _field.min)
                    {
                        SFX.Play("textLetter", 0.7f);
                        _filterField.value = false;
                        return;
                    }
                    if ((bool)_filterField.value && (trigger == Triggers.MenuRight || trigger == Triggers.Select) && (int)_field.value == _field.max)
                    {
                        SFX.Play("textLetter", 0.7f);
                        _filterField.value = false;
                        return;
                    }
                    if (_setting != null && trigger == Triggers.Menu2)
                    {
                        SFX.Play("textLetter", 0.7f);
                        if (_setting.filterMode == FilterMode.GreaterThan)
                        {
                            _setting.filterMode = FilterMode.Equal;
                            return;
                        }
                        if (_setting.filterMode == FilterMode.Equal)
                        {
                            _setting.filterMode = FilterMode.LessThan;
                            return;
                        }
                        if (_setting.filterMode != FilterMode.LessThan)
                            return;
                        _setting.filterMode = FilterMode.GreaterThan;
                        return;
                    }
                }
                int prev = (int)_field.value;
                if (trigger == Triggers.MenuLeft)
                    _field.value = (int)_field.value - GetStep((int)_field.value, false);
                else if (trigger == Triggers.MenuRight || trigger == Triggers.Select)
                    _field.value = (int)_field.value + GetStep((int)_field.value, true);
                int newVal = (int)Maths.Clamp((int)_field.value, _field.min, _field.max);
                if (_upperBoundField != null && newVal > (int)_upperBoundField.value)
                    _upperBoundField.value = newVal;
                if (_lowerBoundField != null && newVal < (int)_lowerBoundField.value)
                    _lowerBoundField.value = newVal;
                if (prev != newVal && _action != null)
                    _action.Activate();
                if (prev != (int)_field.value)
                {
                    SFX.Play("textLetter", 0.7f);
                }
                int dif = newVal - prev;
                _field.value = newVal;
                if (dif > 0)
                {
                    int totalPercent = dif;
                    using (List<FieldBinding>.Enumerator enumerator = percentageGroup.GetEnumerator())
                    {
                    label_37:
                        while (enumerator.MoveNext())
                        {
                            FieldBinding current = enumerator.Current;
                            int step = GetStep((int)_field.value, false);
                            int dec = (int)MathHelper.Min(totalPercent, step);
                            dec = (int)MathHelper.Min(dec, (int)current.value);
                            DevConsole.Log(dec);
                            while (true)
                            {
                                if ((int)current.value > 0 && totalPercent > 0)
                                {
                                    int newPVal = (int)current.value - dec;
                                    current.value = newPVal;
                                    totalPercent -= dec;
                                }
                                else
                                    goto label_37;
                            }
                        }
                    }
                }
                else if (dif < 0)
                {
                    int totalPercent2 = dif;
                    using (List<FieldBinding>.Enumerator enumerator = percentageGroup.GetEnumerator())
                    {
                    label_45:
                        while (enumerator.MoveNext())
                        {
                            FieldBinding current = enumerator.Current;
                            int step = GetStep((int)_field.value, true);
                            int inc = (int)MathHelper.Min(-totalPercent2, step);
                            inc = (int)MathHelper.Min(inc, (int)current.max - (int)current.value);
                            while (true)
                            {
                                if ((int)current.value < (int)current.max && totalPercent2 < 0)
                                {
                                    int newPVal2 = (int)current.value + inc;
                                    current.value = newPVal2;
                                    totalPercent2 += inc;
                                }
                                else
                                    goto label_45;
                            }
                        }
                    }
                }
                if (_textItem == null || newVal < 0 || newVal >= _valueStrings.Count)
                    return;
                _textItem.text = _valueStrings[newVal];
            }
        }
    }
}
