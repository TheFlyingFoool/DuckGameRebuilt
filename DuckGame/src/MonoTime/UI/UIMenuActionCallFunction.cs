namespace DuckGame
{
    public class UIMenuActionCallFunction : UIMenuAction
    {
        private Function _function;

        public UIMenuActionCallFunction(Function f) => _function = f;

        public override void Activate() => _function();

        public delegate void Function();
    }
}
