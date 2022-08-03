// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuActionCloseMenuCallFunction
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIMenuActionCloseMenuCallFunction : UIMenuAction
    {
        private UIComponent _menu;
        private UIMenuActionCloseMenuCallFunction.Function _function;

        public UIMenuActionCloseMenuCallFunction(
          UIComponent menu,
          UIMenuActionCloseMenuCallFunction.Function f)
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
