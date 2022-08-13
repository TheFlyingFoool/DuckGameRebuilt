// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenu
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIMenu : UIBox
    {
        public static bool disabledDraw;
        public static bool globalUILock;
        protected UIDivider _splitter;
        private UIBox _section;
        private UIText _controlText;
        protected string _controlString;
        private InputProfile _controlProfile;

        public void SetBackFunction(UIMenuAction pAction)
        {
            _section._backFunction = pAction;
            _backFunction = pAction;
        }

        public void SetCloseFunction(UIMenuAction pAction)
        {
            _section._closeFunction = pAction;
            _closeFunction = pAction;
        }

        public void SetAcceptFunction(UIMenuAction pAction)
        {
            _section._acceptFunction = pAction;
            _acceptFunction = pAction;
        }

        public void RunBackFunction()
        {
            if (_section._backFunction == null)
                return;
            _section._backFunction.Activate();
        }

        public UIMenu(
          string title,
          float xpos,
          float ypos,
          float wide = -1f,
          float high = -1f,
          string conString = "",
          InputProfile conProfile = null,
          bool tiny = false)
          : base(xpos, ypos, wide, high)
        {
            _controlProfile = conProfile;
            _splitter = new UIDivider(false, 0f, 4f);
            _section = _splitter.rightSection;
            UIText component1 = new UIText(title, Color.White);
            if (tiny)
            {
                BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
                component1.SetFont(f);
            }
            component1.align |= UIAlign.Top;
            _splitter.topSection.Add(component1, true);
            _controlString = conString;
            if (_controlString != "" && _controlString != null)
            {
                UIDivider component2 = new UIDivider(false, 0f, 4f);
                _controlText = new UIText(_controlString, Color.White, heightAdd: 4f, controlProfile: _controlProfile);
                component2.bottomSection.Add(_controlText, true);
                Add(component2, true);
                _section = component2.topSection;
            }
            base.Add(_splitter, true);
        }

        public string title
        {
            get => ((UIText)_splitter.topSection.components[0]).text;
            set => ((UIText)_splitter.topSection.components[0]).text = value;
        }

        public override void SelectLastMenuItem() => _section.SelectLastMenuItem();

        public override void AssignDefaultSelection() => _section.AssignDefaultSelection();

        public override void Add(UIComponent component, bool doAnchor = true)
        {
            _section.Add(component, doAnchor);
            _dirty = true;
        }

        public override void Insert(UIComponent component, int position, bool doAnchor = true)
        {
            _section.Insert(component, position, doAnchor);
            _dirty = true;
        }

        public override void Update()
        {
            if (_controlText != null)
                _controlText.text = _section._hoverControlString != null ? _section._hoverControlString : _controlString;
            base.Update();
        }

        public override void Draw()
        {
            if (!open && !animating)
                return;
            base.Draw();
        }

        public UIComponent AddMatchSetting(MatchSetting m, bool filterMenu, bool enabled = true)
        {
            UIComponent component = null;
            if (m.value is int)
            {
                FieldBinding upperBoundField = null;
                if (m.maxSyncID != null)
                {
                    foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
                    {
                        if (matchSetting.id == m.maxSyncID)
                            upperBoundField = new FieldBinding(matchSetting, "value");
                    }
                }
                FieldBinding lowerBoundField = null;
                if (m.minSyncID != null)
                {
                    foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
                    {
                        if (matchSetting.id == m.minSyncID)
                            lowerBoundField = new FieldBinding(matchSetting, "value");
                    }
                }
                component = new UIMenuItemNumber(m.name, field: new FieldBinding(m, "value", m.min, m.max), step: m.step, upperBoundField: upperBoundField, lowerBoundField: lowerBoundField, append: m.suffix, filterField: (filterMenu ? new FieldBinding(m, "filtered") : null), valStrings: m.valueStrings, setting: m);
                if (m.percentageLinks != null)
                {
                    foreach (string percentageLink in m.percentageLinks)
                    {
                        MatchSetting matchSetting = TeamSelect2.GetMatchSetting(percentageLink);
                        (component as UIMenuItemNumber).percentageGroup.Add(new FieldBinding(matchSetting, "value", matchSetting.min, matchSetting.max, matchSetting.step));
                    }
                }
            }
            else if (m.value is bool)
                component = new UIMenuItemToggle(m.name, field: new FieldBinding(m, "value"), filterBinding: (filterMenu ? new FieldBinding(m, "filtered") : null));
            else if (m.value is string)
                component = new UIMenuItemString(m.name, m.id, field: new FieldBinding(m, "value"), filterBinding: (filterMenu ? new FieldBinding(m, "filtered") : null));
            component.condition = m.condition;
            if (component != null)
            {
                component.isEnabled = enabled;
                _section.Add(component, true);
                _dirty = true;
            }
            return component;
        }

        public override void Remove(UIComponent component)
        {
            _section.Remove(component);
            _dirty = true;
        }
    }
}
