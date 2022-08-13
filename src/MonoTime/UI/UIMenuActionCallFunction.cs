// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMenuActionCallFunction
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIMenuActionCallFunction : UIMenuAction
    {
        private UIMenuActionCallFunction.Function _function;

        public UIMenuActionCallFunction(UIMenuActionCallFunction.Function f) => _function = f;

        public override void Activate() => _function();

        public delegate void Function();
    }
}
