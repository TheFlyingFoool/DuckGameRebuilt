// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuActionOpenMenu
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIMenuActionOpenMenu : UIMenuAction
    {
        private UIComponent _menu;
        private UIComponent _open;

        public UIMenuActionOpenMenu(UIComponent menu, UIComponent open)
        {
            this._menu = menu;
            this._open = open;
        }

        public override void Activate()
        {
            UIComponent pauseMenu = MonoMain.pauseMenu;
            this._menu.Close();
            this._open.Open();
            UIComponent menu = this._menu;
            if (pauseMenu != menu)
                return;
            MonoMain.pauseMenu = this._open;
        }
    }
}
