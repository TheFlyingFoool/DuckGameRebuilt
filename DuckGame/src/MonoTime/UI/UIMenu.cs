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
        public bool gamepadMode = true;
        public bool domouse
        {
            get
            {
                if (!DGRSettings.MenuMouse) return false;
                return _domouse;
            }
            set
            {
                _domouse = value;
            }
        }

        public bool _domouse = true;
        private Vec2 _oldPos;
        private SpriteMap _cursor;
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

        public void SetOpenFunction(UIMenuAction pAction)
        {
            _section._openFunction = pAction;
            _openFunction = pAction;
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
          bool tinyTitle = false)
          : base(xpos, ypos, wide, high)
        {
            _cursor = new SpriteMap("cursors", 16, 16);
            _controlProfile = conProfile;
            _splitter = new UIDivider(false, 0f, 4f);
            _section = _splitter.rightSection;
            UIText titleComponent = new UIText(title, Color.White);
            if (tinyTitle)
            {
                BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
                titleComponent.SetFont(f);
            }
            titleComponent.align |= UIAlign.Center;
            _splitter.topSection.Add(titleComponent, true);
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
            AdjustTitleScale();
        }

        public string title
        {
            get => ((UIText)_splitter.topSection.components[0]).text;
            set
            {
                ((UIText)_splitter.topSection.components[0]).text = value;
                AdjustTitleScale();
            }
        }

        protected void AdjustTitleScale()
        {
            UIText titleComponent = (UIText)_splitter.topSection.components[0];
            titleComponent.scale = new Vec2(1f);
            titleComponent._font.scale = new Vec2(1f);
            for (int i = 0; i < 3; i++)
                if (titleComponent.width > width - borderSize.x * 2f)
                    titleComponent.scale -= new Vec2(0.25f);
                else
                    break;
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
        private InputState previnputState;
        private InputState storeinputState;
        public override void Update()
        {
            if (_controlText != null)
                _controlText.text = _section._hoverControlString != null ? _section._hoverControlString : _controlString;
            previnputState = storeinputState;
            if (domouse)
            {
                if (Input.Pressed(Triggers.Any))
                {
                    gamepadMode = true;
                    _oldPos = Mouse.positionScreen;
                    storeinputState = Mouse.left;
                }
                if (gamepadMode)
                {
                    if ((_oldPos - Mouse.positionScreen).lengthSq > 120)
                    {
                        storeinputState = Mouse.left;
                        gamepadMode = false;
                    }
                }
                if (!gamepadMode)
                {
                    storeinputState = Mouse.left;
                    //Mouse.left previnputState == InputState.Pressed
                }
            }
            else
            {
                gamepadMode = false;
            }
            base.Update();
        }
        public override void Draw()
        {
            if (!open && !animating)
                return;
            if (domouse && Mouse.available && !gamepadMode) //
            {

                _cursor.depth = (Depth)1f;
                _cursor.scale = new Vec2(0.5f, 0.5f);
                _cursor.position = Mouse.position;
                _cursor.frame = 0;
                if (Editor.hoverTextBox)
                {
                    _cursor.frame = 7;
                    _cursor.position.y -= 4f;
                    _cursor.scale = new Vec2(0.25f, 0.5f);
                }
                _cursor.Draw();
            }
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

        public UIComponent AddMatchSettingL(MatchSetting m, bool filterMenu, bool enabled = true)
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
                component = new LUIMenuItemNumber(m.name, field: new FieldBinding(m, "value", m.min, m.max), step: m.step, upperBoundField: upperBoundField, lowerBoundField: lowerBoundField, append: m.suffix, filterField: (filterMenu ? new FieldBinding(m, "filtered") : null), valStrings: m.valueStrings, setting: m);
                if (m.percentageLinks != null)
                {
                    foreach (string percentageLink in m.percentageLinks)
                    {
                        MatchSetting matchSetting = TeamSelect2.GetMatchSetting(percentageLink);
                        (component as LUIMenuItemNumber).percentageGroup.Add(new FieldBinding(matchSetting, "value", matchSetting.min, matchSetting.max, matchSetting.step));
                    }
                }
            }
            else if (m.value is bool)
                component = new LUIMenuItemToggle(m.name, field: new FieldBinding(m, "value"), filterBinding: (filterMenu ? new FieldBinding(m, "filtered") : null));
            else if (m.value is string)
                component = new LUIMenuItemString(m.name, m.id, field: new FieldBinding(m, "value"), filterBinding: (filterMenu ? new FieldBinding(m, "filtered") : null));
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
