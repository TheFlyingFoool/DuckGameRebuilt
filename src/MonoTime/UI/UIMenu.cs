// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenu
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._section._backFunction = pAction;
            this._backFunction = pAction;
        }

        public void SetCloseFunction(UIMenuAction pAction)
        {
            this._section._closeFunction = pAction;
            this._closeFunction = pAction;
        }

        public void SetAcceptFunction(UIMenuAction pAction)
        {
            this._section._acceptFunction = pAction;
            this._acceptFunction = pAction;
        }

        public void RunBackFunction()
        {
            if (this._section._backFunction == null)
                return;
            this._section._backFunction.Activate();
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
            this._controlProfile = conProfile;
            this._splitter = new UIDivider(false, 0.0f, 4f);
            this._section = this._splitter.rightSection;
            UIText component1 = new UIText(title, Color.White);
            if (tiny)
            {
                BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
                component1.SetFont(f);
            }
            component1.align |= UIAlign.Top;
            this._splitter.topSection.Add((UIComponent)component1, true);
            this._controlString = conString;
            if (this._controlString != "" && this._controlString != null)
            {
                UIDivider component2 = new UIDivider(false, 0.0f, 4f);
                this._controlText = new UIText(this._controlString, Color.White, heightAdd: 4f, controlProfile: this._controlProfile);
                component2.bottomSection.Add((UIComponent)this._controlText, true);
                this.Add((UIComponent)component2, true);
                this._section = component2.topSection;
            }
            base.Add((UIComponent)this._splitter, true);
        }

        public string title
        {
            get => ((UIText)this._splitter.topSection.components[0]).text;
            set => ((UIText)this._splitter.topSection.components[0]).text = value;
        }

        public override void SelectLastMenuItem() => this._section.SelectLastMenuItem();

        public override void AssignDefaultSelection() => this._section.AssignDefaultSelection();

        public override void Add(UIComponent component, bool doAnchor = true)
        {
            this._section.Add(component, doAnchor);
            this._dirty = true;
        }

        public override void Insert(UIComponent component, int position, bool doAnchor = true)
        {
            this._section.Insert(component, position, doAnchor);
            this._dirty = true;
        }

        public override void Update()
        {
            if (this._controlText != null)
                this._controlText.text = this._section._hoverControlString != null ? this._section._hoverControlString : this._controlString;
            base.Update();
        }

        public override void Draw()
        {
            if (!this.open && !this.animating)
                return;
            base.Draw();
        }

        public UIComponent AddMatchSetting(MatchSetting m, bool filterMenu, bool enabled = true)
        {
            UIComponent component = (UIComponent)null;
            if (m.value is int)
            {
                FieldBinding upperBoundField = (FieldBinding)null;
                if (m.maxSyncID != null)
                {
                    foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
                    {
                        if (matchSetting.id == m.maxSyncID)
                            upperBoundField = new FieldBinding((object)matchSetting, "value");
                    }
                }
                FieldBinding lowerBoundField = (FieldBinding)null;
                if (m.minSyncID != null)
                {
                    foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
                    {
                        if (matchSetting.id == m.minSyncID)
                            lowerBoundField = new FieldBinding((object)matchSetting, "value");
                    }
                }
                component = (UIComponent)new UIMenuItemNumber(m.name, field: new FieldBinding((object)m, "value", (float)m.min, (float)m.max), step: m.step, upperBoundField: upperBoundField, lowerBoundField: lowerBoundField, append: m.suffix, filterField: (filterMenu ? new FieldBinding((object)m, "filtered") : (FieldBinding)null), valStrings: m.valueStrings, setting: m);
                if (m.percentageLinks != null)
                {
                    foreach (string percentageLink in m.percentageLinks)
                    {
                        MatchSetting matchSetting = TeamSelect2.GetMatchSetting(percentageLink);
                        (component as UIMenuItemNumber).percentageGroup.Add(new FieldBinding((object)matchSetting, "value", (float)matchSetting.min, (float)matchSetting.max, (float)matchSetting.step));
                    }
                }
            }
            else if (m.value is bool)
                component = (UIComponent)new UIMenuItemToggle(m.name, field: new FieldBinding((object)m, "value"), filterBinding: (filterMenu ? new FieldBinding((object)m, "filtered") : (FieldBinding)null));
            else if (m.value is string)
                component = (UIComponent)new UIMenuItemString(m.name, m.id, field: new FieldBinding((object)m, "value"), filterBinding: (filterMenu ? new FieldBinding((object)m, "filtered") : (FieldBinding)null));
            component.condition = m.condition;
            if (component != null)
            {
                component.isEnabled = enabled;
                this._section.Add(component, true);
                this._dirty = true;
            }
            return component;
        }

        public override void Remove(UIComponent component)
        {
            this._section.Remove(component);
            this._dirty = true;
        }
    }
}
