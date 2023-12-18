namespace DuckGame
{
    public class UIMenuActionChangeLevel : UIMenuAction
    {
        private UIComponent _menu;
        private Level _destination;
        private bool _activated;

        public UIMenuActionChangeLevel(UIComponent menu, Level destination)
        {
            _menu = menu;
            _destination = destination;
        }

        public override void Update()
        {
            if (!_activated)
                return;
            Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.09f);
            if (Graphics.fade != 0)
                return;
            Level.current = _destination;
        }

        public override void Activate() => _activated = true;
    }
}
