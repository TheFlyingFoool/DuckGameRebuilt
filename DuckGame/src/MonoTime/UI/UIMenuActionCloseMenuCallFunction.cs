namespace DuckGame
{
    public class UIMenuActionCloseMenuCallFunction : UIMenuAction
    {
        private UIComponent _menu;
        private Function _function;

        public UIMenuActionCloseMenuCallFunction(
          UIComponent menu,
          Function f)
        {
            _menu = menu;
            _function = f;
        }

        public override void Activate()
        {
            _menu.Close();
            _function();
        }

        public delegate void Function();
    }
}
