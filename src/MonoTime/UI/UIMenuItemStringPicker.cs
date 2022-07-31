// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuItemStringPicker
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._valueStrings = valStrings;
            UIDivider component1 = new UIDivider(true, this._valueStrings != null ? 0f : 0.8f);
            UIText component2 = new UIText(text, c)
            {
                align = UIAlign.Left
            };
            component1.leftSection.Add(component2, true);
            this.currentIndex = this._valueStrings.IndexOf(field.value as string);
            if (this.currentIndex < 0)
                this.currentIndex = 0;
            this._textItem = new UIChangingText(-1f, -1f, field, null);
            string str = "";
            foreach (string valString in valStrings)
            {
                if (valString.Length > str.Length)
                    str = valString;
            }
          (this._textItem as UIChangingText).defaultSizeString = str;
            this._textItem.minLength = str.Length;
            this._textItem.text = this._valueStrings[this.currentIndex];
            this._textItem.align = UIAlign.Right;
            component1.rightSection.Add(_textItem, true);
            this.rightSection.Add(component1, true);
            this._arrow = new UIImage("contextArrowRight")
            {
                align = UIAlign.Right,
                visible = false
            };
            this.leftSection.Add(_arrow, true);
            this._field = field;
            this.currentValue = this._field.value as string;
            this.controlString = "@WASD@ADJUST @SELECT@APPLY";
        }

        public override void Activate(string trigger)
        {
            if (this._useBaseActivationLogic)
                base.Activate(trigger);
            else if (trigger == "SELECT")
            {
                if (this.selectAction != null)
                    this.selectAction(this.currentValue);
                this._field.value = currentValue;
            }
            else
            {
                int currentIndex1 = this.currentIndex;
                if (trigger == "MENULEFT")
                    --this.currentIndex;
                else if (trigger == "MENURIGHT")
                    ++this.currentIndex;
                if (this.currentIndex < 0)
                    this.currentIndex = 0;
                if (this.currentIndex > this._valueStrings.Count - 1)
                    this.currentIndex = this._valueStrings.Count - 1;
                this.currentValue = this._valueStrings[this.currentIndex];
                int currentIndex2 = this.currentIndex;
                if (currentIndex1 != currentIndex2)
                    SFX.Play("textLetter", 0.7f);
                if (this._textItem == null)
                    return;
                this._textItem.text = this._valueStrings[this.currentIndex];
            }
        }

        public override void Draw()
        {
            if (!this.selected)
                this.currentValue = this._field.value as string;
            this.currentIndex = this._valueStrings.IndexOf(this.currentValue);
            if (this.currentIndex < 0)
                this.currentIndex = 0;
            if (!this._textItem.text.Contains(this._valueStrings[this.currentIndex]))
                this._textItem.text = this._valueStrings[this.currentIndex];
            base.Draw();
        }
    }
}
