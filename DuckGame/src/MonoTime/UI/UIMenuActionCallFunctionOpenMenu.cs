// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuActionCallFunctionOpenMenu
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIMenuActionCallFunctionOpenMenu : UIMenuAction
    {
        private UIComponent _menu;
        private UIComponent _open;
        private UIMenuActionCallFunctionOpenMenu.Function _function;

        public UIMenuActionCallFunctionOpenMenu(
          UIComponent menu,
          UIComponent open,
          UIMenuActionCallFunctionOpenMenu.Function f)
        {
            _menu = menu;
            _open = open;
            _function = f;
        }

        public override void Activate()
        {
            _function();
            _menu.Close();
            _open.Open();
            if (MonoMain.pauseMenu != _menu && (MonoMain.pauseMenu == null || !(MonoMain.pauseMenu.GetType() != typeof(UIComponent))) && MonoMain.pauseMenu != null)
                return;
            MonoMain.pauseMenu = _open;
        }

        public delegate void Function();
    }
}
