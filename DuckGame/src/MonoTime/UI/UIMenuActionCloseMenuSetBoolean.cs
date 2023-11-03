namespace DuckGame
{
    public class UIMenuActionCloseMenuSetBoolean : UIMenuAction
    {
        private UIComponent _menu;
        private MenuBoolean _value;

        public UIMenuActionCloseMenuSetBoolean(UIComponent menu, MenuBoolean value)
        {
            _menu = menu;
            _value = value;
        }

        public override void Activate()
        {
            _menu.Close();
            _value.value = true;
        }
    }
}
