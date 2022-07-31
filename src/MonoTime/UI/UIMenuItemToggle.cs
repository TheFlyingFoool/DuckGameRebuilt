// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuItemToggle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class UIMenuItemToggle : UIMenuItem
    {
        private FieldBinding _field;
        private FieldBinding _filterBinding;
        private bool _compressed;
        private List<string> _multiToggle;
        private UIMultiToggle _multiToggleElement;

        public void SetFieldBinding(FieldBinding f)
        {
            this._field = f;
            if (this._multiToggleElement == null)
                return;
            this._multiToggleElement.SetFieldBinding(f);
        }

        public UIMenuItemToggle(
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
            UIDivider component1 = new UIDivider(true, 0f);
            if (text != "")
            {
                UIText component2 = new UIText(text, c);
                if (tiny)
                    component2.SetFont(f);
                component2.align = UIAlign.Left;
                component1.leftSection.Add(component2, true);
            }
            if (multi != null)
            {
                this._multiToggleElement = new UIMultiToggle(-1f, -1f, field, multi, compressedMulti)
                {
                    align = compressedMulti ? UIAlign.Right : UIAlign.Right
                };
                if (text != "")
                {
                    component1.rightSection.Add(_multiToggleElement, true);
                }
                else
                {
                    component1.leftSection.Add(_multiToggleElement, true);
                    this._multiToggleElement.align = UIAlign.Left;
                }
                if (tiny)
                    this._multiToggleElement.SetFont(f);
                this._multiToggle = multi;
                this._compressed = compressedMulti;
            }
            else
            {
                UIOnOff component3 = new UIOnOff(-1f, -1f, field, filterBinding);
                if (tiny)
                    component3.SetFont(f);
                component3.align = UIAlign.Right;
                component1.rightSection.Add(component3, true);
            }
            this.rightSection.Add(component1, true);
            if (tiny)
                this._arrow = new UIImage("littleContextArrowRight");
            else
                this._arrow = new UIImage("contextArrowRight");
            this._arrow.align = UIAlign.Right;
            this._arrow.visible = false;
            this.leftSection.Add(_arrow, true);
            this._field = field;
            this._filterBinding = filterBinding;
            this.controlString = "@CANCEL@BACK @WASD@ADJUST";
        }

        public override void Activate(string trigger)
        {
            int num1 = 1;
            int num2 = this._filterBinding != null ? -1 : 0;
            int num3;
            if (this._multiToggle != null)
            {
                num1 = this._multiToggle.Count - 1;
                num3 = (int)this._field.value;
            }
            else
                num3 = (bool)this._field.value ? 1 : 0;
            if (this._filterBinding != null && !(bool)this._filterBinding.value)
                num3 = -1;
            bool flag = false;
            if (trigger == "SELECT" || trigger == "MENURIGHT")
            {
                ++num3;
                flag = true;
            }
            else if (trigger == "MENULEFT")
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
                this._filterBinding.value = false;
            }
            else
            {
                if (this._filterBinding != null)
                    this._filterBinding.value = true;
                this._field.value = this._multiToggle == null ? num3 != 0 : num3;
            }
            if (!flag)
                return;
            SFX.Play("textLetter", 0.7f);
            if (this._action == null)
                return;
            this._action.Activate();
        }
    }
}
