using System.Collections.Generic;

namespace DuckGame
{
    public class UIMenuItemNumber : UIMenuItem
    {
        protected FieldBinding _field;
        protected int _step;
        protected FieldBinding _upperBoundField;
        protected FieldBinding _lowerBoundField;
        protected FieldBinding _filterField;
        protected UIText _textItem;
        public List<FieldBinding> percentageGroup = new List<FieldBinding>();
        private List<string> _valueStrings;
        private MatchSetting _setting;
        protected bool _useBaseActivationLogic;

        public UIMenuItemNumber(
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
            UIText t = new UIText(text, c)
            {
                align = UIAlign.Left
            };
            splitter.leftSection.Add(t, true);
            if (field == null)
            {
                _textItem = new UIChangingText(-1f, -1f, field, null)
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
                    _textItem = new UIChangingText(-1f, -1f, field, null);
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
              (_textItem as UIChangingText).defaultSizeString = str + "   ";
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
            int step1 = 0;
            foreach (KeyValuePair<int, int> step2 in _setting.stepMap)
            {
                step1 = step2.Value;
                if (up)
                {
                    if (step2.Key > current)
                        break;
                }
                if (!up)
                {
                    if (step2.Key >= current)
                        break;
                }
            }
            return step1;
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
                        SFX.DontSave = 1;
                        SFX.Play("textLetter", 0.7f);
                        _filterField.value = true;
                        _field.value = (int)_field.min;
                        return;
                    }
                    if (!(bool)_filterField.value && trigger == Triggers.MenuLeft)
                    {
                        SFX.DontSave = 1;
                        SFX.Play("textLetter", 0.7f);
                        _filterField.value = true;
                        _field.value = (int)_field.max;
                        return;
                    }
                    if ((bool)_filterField.value && trigger == Triggers.MenuLeft && (int)_field.value == _field.min)
                    {
                        SFX.DontSave = 1;
                        SFX.Play("textLetter", 0.7f);
                        _filterField.value = false;
                        return;
                    }
                    if ((bool)_filterField.value && (trigger == Triggers.MenuRight || trigger == Triggers.Select) && (int)_field.value == _field.max)
                    {
                        SFX.DontSave = 1;
                        SFX.Play("textLetter", 0.7f);
                        _filterField.value = false;
                        return;
                    }
                    if (_setting != null && trigger == Triggers.Menu2)
                    {
                        SFX.DontSave = 1;
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
                int num1 = (int)_field.value;
                if (trigger == Triggers.MenuLeft)
                    _field.value = (int)_field.value - GetStep((int)_field.value, false);
                else if (trigger == Triggers.MenuRight || trigger == Triggers.Select)
                    _field.value = (int)_field.value + GetStep((int)_field.value, true);
                int index = (int)Maths.Clamp((int)_field.value, _field.min, _field.max);
                if (_upperBoundField != null && index > (int)_upperBoundField.value)
                    _upperBoundField.value = index;
                if (_lowerBoundField != null && index < (int)_lowerBoundField.value)
                    _lowerBoundField.value = index;
                if (num1 != index && _action != null)
                    _action.Activate();
                if (num1 != (int)_field.value)
                {
                    SFX.DontSave = 1;
                    SFX.Play("textLetter", 0.7f);
                }
                int num2 = index - num1;
                _field.value = index;
                if (num2 > 0)
                {
                    int num3 = num2;
                    using (List<FieldBinding>.Enumerator enumerator = percentageGroup.GetEnumerator())
                    {
                    label_37:
                        while (enumerator.MoveNext())
                        {
                            FieldBinding current = enumerator.Current;
                            while (true)
                            {
                                if ((int)current.value > current.min && num3 > 0)
                                {
                                    int num4 = (int)current.value - (int)current.inc;
                                    current.value = num4;
                                    num3 -= (int)current.inc;
                                }
                                else
                                    goto label_37;
                            }
                        }
                    }
                }
                else if (num2 < 0)
                {
                    int num5 = num2;
                    using (List<FieldBinding>.Enumerator enumerator = percentageGroup.GetEnumerator())
                    {
                    label_45:
                        while (enumerator.MoveNext())
                        {
                            FieldBinding current = enumerator.Current;
                            while (true)
                            {
                                if ((int)current.value < current.max && num5 < 0)
                                {
                                    int num6 = (int)current.value + (int)current.inc;
                                    current.value = num6;
                                    num5 += (int)current.inc;
                                }
                                else
                                    goto label_45;
                            }
                        }
                    }
                }
                if (_textItem == null || index < 0 || index >= _valueStrings.Count)
                    return;
                _textItem.text = _valueStrings[index];
            }
        }
    }
}