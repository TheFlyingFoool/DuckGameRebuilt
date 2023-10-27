namespace DuckGame
{
    public class UIMenuActionOpenMenuSetBoolean : UIMenuActionOpenMenu
    {
        //private UIComponent _menu;
        //private UIComponent _open;
        private MenuBoolean _value;

        public UIMenuActionOpenMenuSetBoolean(UIComponent menu, UIComponent open, MenuBoolean value)
          : base(menu, open)
        {
            _value = value;
        }

        public override void Activate()
        {
            base.Activate();
            _value.value = true;
        }
    }
}
