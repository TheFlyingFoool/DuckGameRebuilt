// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuItemString
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        private UIStringEntry _stringEntry;
        private UIMenuActionOpenMenu _activateFunction;

        public void SetFieldBinding(FieldBinding f) => _field = f;

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
            _text = text;
            if (c == new Color())
                c = Colors.MenuOption;
            _id = id;
            BitmapFont f = null;
            if (tiny)
                f = new BitmapFont("smallBiosFontUI", 7, 5);
            UIDivider splitter = new UIDivider(true, 0f);
            if (text != "")
            {
                UIText t = new UIText(text, c, al: UIAlign.Left);
                if (tiny)
                    t.SetFont(f);
                splitter.leftSection.Add(t, true);
            }
            _stringEntry = new UIStringEntry(false, "", Color.White, al: UIAlign.Right);
            splitter.rightSection.Add(_stringEntry, true);
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

        public override void Update()
        {
            if ((string)_field.value == "")
            {
                if (open && _id == "name" && Profiles.active.Count > 0)
                {
                    _field.value = TeamSelect2.DefaultGameName();
                    _stringEntry.text = (string)_field.value;
                }
                else
                    _stringEntry.text = "NONE";
            }
            else
                _stringEntry.text = (string)_field.value;
            base.Update();
        }

        public void InitializeEntryMenu(UIComponent pGroup, UIMenu pReturn)
        {
            _enterStringMenu = !(_id == "port") ? new UIStringEntryMenu(false, "SET " + _text, _field) : new UIStringEntryMenu(false, "SET " + _text, _field, 6, true, 1337, 55535);
            _enterStringMenu.SetBackFunction(new UIMenuActionOpenMenu(_enterStringMenu, pReturn));
            _enterStringMenu.SetAcceptFunction(new UIMenuActionCallFunction(() =>
            {
                if (_id == "name" && (string)_field.value == "")
                    _field.value = TeamSelect2.DefaultGameName();
                if (Network.activeNetwork.core.lobby is not null)
                {
                    Network.activeNetwork.core.lobby.SetLobbyData(_id, (string)_field.value);
                    if (_id == "name")
                    {
                        Network.activeNetwork.core.lobby.SetLobbyData("customName", (string)_field.value != TeamSelect2.DefaultGameName() ? "true" : "false");
                        if (DGRSettings.LobbyNameOnPause)
                            DuckNetwork.core._ducknetMenu.title = (string)_field.value;
                    }
                }
            }));
            _enterStringMenu.Close();
            pGroup.Add(_enterStringMenu, false);
            _activateFunction = new UIMenuActionOpenMenu(pReturn, _enterStringMenu);
        }

        public override void Activate(string trigger)
        {
            if (!(trigger == Triggers.Select))
                return;
            _enterStringMenu.SetValue((string)_field.value);
            _activateFunction.Activate();
        }
    }
}
