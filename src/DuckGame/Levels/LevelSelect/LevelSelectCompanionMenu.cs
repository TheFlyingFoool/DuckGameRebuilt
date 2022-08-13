// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelSelectCompanionMenu
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _returnMenu = returnMenu;
        }

        public override void Initialize() => base.Initialize();

        public override void Open()
        {
            if (!LevelSelect._skipCompanionOpening)
            {
                _levelSelector = new LevelSelect(returnMenu: this);
                _levelSelector.Initialize();
                Editor.selectingLevel = true;
                _justOpened = true;
            }
            else
                _levelSelector.HUDRefresh();
            LevelSelect._skipCompanionOpening = false;
            base.Open();
        }

        public override void Update()
        {
            if (open)
            {
                if (!_justOpened)
                {
                    _levelSelector.Update();
                    if (_levelSelector.isClosed)
                    {
                        Editor.selectingLevel = false;
                        _levelSelector.Terminate();
                        new UIMenuActionOpenMenu(this, _returnMenu).Activate();
                        return;
                    }
                }
                _justOpened = false;
            }
            base.Update();
        }

        public override void Draw()
        {
            if (!open)
                return;
            _levelSelector.DrawThings(true);
        }
    }
}
