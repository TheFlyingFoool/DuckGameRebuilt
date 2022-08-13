// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuActionOpenMenuCallFunction
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIMenuActionOpenMenuCallFunction : UIMenuAction
    {
        private UIComponent _menu;
        private UIComponent _open;
        private UIMenuActionOpenMenuCallFunction.Function _function;

        public UIMenuActionOpenMenuCallFunction(
          UIComponent menu,
          UIComponent open,
          UIMenuActionOpenMenuCallFunction.Function f)
        {
            _menu = menu;
            _open = open;
            _function = f;
        }

        public override void Activate()
        {
            _menu.Close();
            _open.Open();
            if (MonoMain.pauseMenu == _menu || MonoMain.pauseMenu != null && MonoMain.pauseMenu.GetType() != typeof(UIComponent) || MonoMain.pauseMenu == null)
                MonoMain.pauseMenu = _open;
            _function();
        }

        public delegate void Function();
    }
}
