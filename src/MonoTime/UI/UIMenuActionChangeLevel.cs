// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuActionChangeLevel
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            if (Graphics.fade != 0.0)
                return;
            Level.current = _destination;
        }

        public override void Activate() => _activated = true;
    }
}
