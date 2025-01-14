namespace DuckGame
{
    public class UIMenuActionCloseMenu : UIMenuAction
    {
        private UIComponent _menu;

        public UIMenuActionCloseMenu(UIComponent menu) => _menu = menu;

        public override void Activate() => _menu.Close();
    }
}
