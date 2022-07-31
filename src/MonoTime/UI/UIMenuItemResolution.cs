// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuItemResolution
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
                this.controlString = "@WASD@ADJUST @SELECT@APPLY";
            else
                this.controlString = "@WASD@ADJUST @SELECT@APPLY @MENU2@ALL";
            UIDivider component1 = new UIDivider(true, 0f);
            UIText component2 = new UIText(text, c)
            {
                align = UIAlign.Left
            };
            component1.leftSection.Add(component2, true);
            this.RefreshValueList();
            this.currentIndex = this._values.IndexOf(field.value as Resolution);
            if (this.currentIndex < 0)
                this.currentIndex = 0;
            this._textItem = new UIChangingText(-1f, -1f, field, null);
            string str = "";
            foreach (Resolution resolution in this._values)
            {
                if (resolution.ToShortString().Length > str.Length)
                    str = resolution.ToShortString();
            }
          (this._textItem as UIChangingText).defaultSizeString = str + "   ";
            this._textItem.minLength = str.Length + 3;
            this._textItem.text = this._values[this.currentIndex].ToShortString() + "   ";
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
            this.currentValue = this._field.value as Resolution;
        }

        private void RefreshValueList()
        {
            if (this.showAll)
            {
                this.controlString = "@WASD@ADJUST @SELECT@APPLY @MENU2@BASIC";
                this._values = Resolution.supportedDisplaySizes[Resolution.current.mode];
            }
            else
            {
                this.controlString = "@WASD@ADJUST @SELECT@APPLY @MENU2@ALL";
                if (Resolution.current.mode == ScreenMode.Windowed)
                    this._values = Resolution.supportedDisplaySizes[Resolution.current.mode].Where<Resolution>(x => x.recommended || x == Resolution.current).ToList<Resolution>();
                else
                    this._values = Resolution.supportedDisplaySizes[Resolution.current.mode].Where<Resolution>(x => (double)Math.Abs(x.aspect - Resolution.adapterResolution.aspect) < 0.0500000007450581 || x == Resolution.current).ToList<Resolution>();
            }
        }

        public override void Activate(string trigger)
        {
            if (trigger == "SELECT")
            {
                this._field.value = currentValue;
                if (this.selectAction == null)
                    return;
                this.selectAction();
            }
            else if (trigger == "MENU2")
            {
                this.showAll = !this.showAll;
                this.RefreshValueList();
                SFX.Play("textLetter", 0.7f);
                this.currentIndex = this._values.IndexOf(this._field.value as Resolution);
                if (this.currentIndex < 0)
                    this.currentIndex = 0;
                this._textItem.text = this._values[this.currentIndex].ToShortString();
            }
            else if (trigger == "SELECT")
            {
                if (this.selectAction != null)
                    this.selectAction();
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
                if (this.currentIndex > this._values.Count - 1)
                    this.currentIndex = this._values.Count - 1;
                this.currentValue = this._values[this.currentIndex];
                int currentIndex2 = this.currentIndex;
                if (currentIndex1 != currentIndex2)
                    SFX.Play("textLetter", 0.7f);
                if (this._textItem == null)
                    return;
                this._textItem.text = this._values[this.currentIndex].ToShortString();
            }
        }

        public override void Draw()
        {
            if (Resolution.current != this._current)
            {
                this.RefreshValueList();
                this._current = Resolution.current;
            }
            if (!this.selected)
                this.currentValue = this._field.value as Resolution;
            this.currentIndex = this._values.IndexOf(this.currentValue);
            if (this.currentIndex < 0)
                this.currentIndex = this._values.Count - 1;
            this._textItem.text = this._values[this.currentIndex].ToShortString();
            base.Draw();
        }
    }
}
