namespace DuckGame
{
    public class UIMenuActionCallFunctionOpenMenu : UIMenuAction
    {
        private UIComponent _menu;
        private UIComponent _open;
        private Function _function;

        public UIMenuActionCallFunctionOpenMenu(
          UIComponent menu,
          UIComponent open,
          Function f)
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
