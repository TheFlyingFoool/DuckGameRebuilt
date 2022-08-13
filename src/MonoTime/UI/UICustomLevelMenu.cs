// Decompiled with JetBrains decompiler
// Type: DuckGame.UICustomLevelMenu
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UICustomLevelMenu : UIMenuItemNumber
    {
        public UICustomLevelMenu(UIMenuAction action = null, UIAlign al = UIAlign.Center, Color c = default(Color))
          : base("CUSTOM LEVELS", action, step: 0, c: c)
        {
            _useBaseActivationLogic = true;
            controlString = null;
        }

        public override void Update()
        {
            int num = 0;
            foreach (string activatedLevel in Editor.activatedLevels)
                ++num;
            if (_textItem != null)
            {
                if (num == 0)
                    _textItem.text = "NONE";
                else
                    _textItem.text = num.ToString();
            }
            base.Update();
        }
    }
}
