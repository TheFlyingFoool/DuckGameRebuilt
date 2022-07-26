// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelSelectCompanionMenu
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class LevelSelectCompanionMenu : UIMenu
    {
        private LevelSelect _levelSelector;
        private UIMenu _returnMenu;
        private bool _justOpened;

        public LevelSelectCompanionMenu(float xpos, float ypos, UIMenu returnMenu)
          : base("", xpos, ypos)
        {
            this._returnMenu = returnMenu;
        }

        public override void Initialize() => base.Initialize();

        public override void Open()
        {
            if (!LevelSelect._skipCompanionOpening)
            {
                this._levelSelector = new LevelSelect(returnMenu: ((UIMenu)this));
                this._levelSelector.Initialize();
                Editor.selectingLevel = true;
                this._justOpened = true;
            }
            else
                this._levelSelector.HUDRefresh();
            LevelSelect._skipCompanionOpening = false;
            base.Open();
        }

        public override void Update()
        {
            if (this.open)
            {
                if (!this._justOpened)
                {
                    this._levelSelector.Update();
                    if (this._levelSelector.isClosed)
                    {
                        Editor.selectingLevel = false;
                        this._levelSelector.Terminate();
                        new UIMenuActionOpenMenu((UIComponent)this, (UIComponent)this._returnMenu).Activate();
                        return;
                    }
                }
                this._justOpened = false;
            }
            base.Update();
        }

        public override void Draw()
        {
            if (!this.open)
                return;
            this._levelSelector.DrawThings(true);
        }
    }
}
