// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuActionCloseMenuSetBoolean
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIMenuActionCloseMenuSetBoolean : UIMenuAction
    {
        private UIComponent _menu;
        private MenuBoolean _value;

        public UIMenuActionCloseMenuSetBoolean(UIComponent menu, MenuBoolean value)
        {
            this._menu = menu;
            this._value = value;
        }

        public override void Activate()
        {
            this._menu.Close();
            this._value.value = true;
        }
    }
}
