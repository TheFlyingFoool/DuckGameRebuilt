// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuItemNumber
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this._setting = setting;
            if (c == new Color())
                c = Colors.MenuOption;
            this._valueStrings = valStrings;
            UIDivider component1 = new UIDivider(true, this._valueStrings != null ? 0f : 0.8f);
            UIText component2 = new UIText(text, c)
            {
                align = UIAlign.Left
            };
            component1.leftSection.Add(component2, true);
            if (field == null)
            {
                this._textItem = new UIChangingText(-1f, -1f, field, null);
                this._textItem.align = UIAlign.Right;
                component1.rightSection.Add(_textItem, true);
            }
            else if (this._valueStrings != null)
            {
                if (text == "" || text == null)
                {
                    component1.leftSection.align = UIAlign.Left;
                    this._textItem = component2;
                    int index = (int)field.value;
                    if (index >= 0 && index < this._valueStrings.Count)
                        this._textItem.text = this._valueStrings[index];
                }
                else
                {
                    this._textItem = new UIChangingText(-1f, -1f, field, null);
                    int index = (int)field.value;
                    if (index >= 0 && index < this._valueStrings.Count)
                        this._textItem.text = this._valueStrings[index];
                    this._textItem.align = UIAlign.Right;
                    component1.rightSection.Add(_textItem, true);
                }
            }
            else
            {
                UINumber component3 = new UINumber(-1f, -1f, field, append, filterField, this._setting)
                {
                    align = UIAlign.Right
                };
                component1.rightSection.Add(component3, true);
            }
            if (this._valueStrings != null)
            {
                string str = "";
                foreach (string valueString in this._valueStrings)
                {
                    if (valueString.Length > str.Length)
                        str = valueString;
                }
              (this._textItem as UIChangingText).defaultSizeString = str + "   ";
                this._textItem.minLength = str.Length + 3;
                this._textItem.text = this._textItem.text;
            }
            this.rightSection.Add(component1, true);
            this._arrow = new UIImage("contextArrowRight")
            {
                align = UIAlign.Right,
                visible = false
            };
            this.leftSection.Add(_arrow, true);
            this._field = field;
            this._step = step;
            this._upperBoundField = upperBoundField;
            this._lowerBoundField = lowerBoundField;
            this._filterField = filterField;
            this.controlString = "@CANCEL@BACK @WASD@ADJUST";
        }

        private int GetStep(int current, bool up)
        {
            if (this._setting == null || this._setting.stepMap == null)
                return this._step;
            int step1 = 0;
            foreach (KeyValuePair<int, int> step2 in this._setting.stepMap)
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
            if (this._useBaseActivationLogic)
            {
                base.Activate(trigger);
            }
            else
            {
                if (this._filterField != null)
                {
                    if (!(bool)this._filterField.value && (trigger == "MENURIGHT" || trigger == "SELECT"))
                    {
                        SFX.Play("textLetter", 0.7f);
                        this._filterField.value = true;
                        this._field.value = (int)this._field.min;
                        return;
                    }
                    if (!(bool)this._filterField.value && trigger == "MENULEFT")
                    {
                        SFX.Play("textLetter", 0.7f);
                        this._filterField.value = true;
                        this._field.value = (int)this._field.max;
                        return;
                    }
                    if ((bool)this._filterField.value && trigger == "MENULEFT" && (int)this._field.value == this._field.min)
                    {
                        SFX.Play("textLetter", 0.7f);
                        this._filterField.value = false;
                        return;
                    }
                    if ((bool)this._filterField.value && (trigger == "MENURIGHT" || trigger == "SELECT") && (int)this._field.value == this._field.max)
                    {
                        SFX.Play("textLetter", 0.7f);
                        this._filterField.value = false;
                        return;
                    }
                    if (this._setting != null && trigger == "MENU2")
                    {
                        SFX.Play("textLetter", 0.7f);
                        if (this._setting.filterMode == FilterMode.GreaterThan)
                        {
                            this._setting.filterMode = FilterMode.Equal;
                            return;
                        }
                        if (this._setting.filterMode == FilterMode.Equal)
                        {
                            this._setting.filterMode = FilterMode.LessThan;
                            return;
                        }
                        if (this._setting.filterMode != FilterMode.LessThan)
                            return;
                        this._setting.filterMode = FilterMode.GreaterThan;
                        return;
                    }
                }
                int num1 = (int)this._field.value;
                if (trigger == "MENULEFT")
                    this._field.value = (int)this._field.value - this.GetStep((int)this._field.value, false);
                else if (trigger == "MENURIGHT" || trigger == "SELECT")
                    this._field.value = (int)this._field.value + this.GetStep((int)this._field.value, true);
                int index = (int)Maths.Clamp((int)this._field.value, this._field.min, this._field.max);
                if (this._upperBoundField != null && index > (int)this._upperBoundField.value)
                    this._upperBoundField.value = index;
                if (this._lowerBoundField != null && index < (int)this._lowerBoundField.value)
                    this._lowerBoundField.value = index;
                if (num1 != index && this._action != null)
                    this._action.Activate();
                if (num1 != (int)this._field.value)
                    SFX.Play("textLetter", 0.7f);
                int num2 = index - num1;
                this._field.value = index;
                if (num2 > 0)
                {
                    int num3 = num2;
                    using (List<FieldBinding>.Enumerator enumerator = this.percentageGroup.GetEnumerator())
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
                    using (List<FieldBinding>.Enumerator enumerator = this.percentageGroup.GetEnumerator())
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
                if (this._textItem == null || index < 0 || index >= this._valueStrings.Count)
                    return;
                this._textItem.text = this._valueStrings[index];
            }
        }
    }
}
