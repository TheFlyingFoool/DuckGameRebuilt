// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuItemString
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIMenuItemString : UIMenuItem
    {
        private FieldBinding _field;
        private FieldBinding _filterBinding;
        private UIStringEntryMenu _enterStringMenu;
        private string _text;
        private string _id;
        private UIStringEntry _passwordItem;
        private UIMenuActionOpenMenu _activateFunction;

        public void SetFieldBinding(FieldBinding f) => this._field = f;

        public UIMenuItemString(
          string text,
          string id,
          UIMenuAction action = null,
          FieldBinding field = null,
          Color c = default(Color),
          FieldBinding filterBinding = null,
          bool tiny = false)
          : base(action)
        {
            this._text = text;
            if (c == new Color())
                c = Colors.MenuOption;
            this._id = id;
            BitmapFont f = null;
            if (tiny)
                f = new BitmapFont("smallBiosFontUI", 7, 5);
            UIDivider component1 = new UIDivider(true, 0.0f);
            if (text != "")
            {
                UIText component2 = new UIText(text, c);
                if (tiny)
                    component2.SetFont(f);
                component2.align = UIAlign.Left;
                component1.leftSection.Add(component2, true);
            }
            this._passwordItem = new UIStringEntry(false, "", Color.White)
            {
                align = UIAlign.Right
            };
            component1.rightSection.Add(_passwordItem, true);
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

        public override void Update()
        {
            if ((string)this._field.value == "")
            {
                if (this.open && this._id == "name" && Profiles.active.Count > 0)
                {
                    this._field.value = TeamSelect2.DefaultGameName();
                    this._passwordItem.text = (string)this._field.value;
                }
                else
                    this._passwordItem.text = "NONE";
            }
            else
                this._passwordItem.text = (string)this._field.value;
            base.Update();
        }

        public void InitializeEntryMenu(UIComponent pGroup, UIMenu pReturn)
        {
            this._enterStringMenu = !(this._id == "port") ? new UIStringEntryMenu(false, "SET " + this._text, this._field) : new UIStringEntryMenu(false, "SET " + this._text, this._field, 6, true, 1337, 55535);
            this._enterStringMenu.SetBackFunction(new UIMenuActionOpenMenu(_enterStringMenu, pReturn));
            this._enterStringMenu.Close();
            pGroup.Add(_enterStringMenu, false);
            this._activateFunction = new UIMenuActionOpenMenu(pReturn, _enterStringMenu);
        }

        public override void Activate(string trigger)
        {
            if (!(trigger == "SELECT"))
                return;
            this._enterStringMenu.SetValue((string)this._field.value);
            this._activateFunction.Activate();
        }
    }
}
