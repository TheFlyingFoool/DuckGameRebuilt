// Decompiled with JetBrains decompiler
// Type: DuckGame.UIFilterMenuItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIFilterMenuItem : UIMenuItem
    {
        public UIFilterMenuItem(UIMenuAction action = null, UIAlign al = UIAlign.Center, Color c = default(Color), bool backButton = false)
          : base("AAAAAAAAAAAAAAAAAA", action, al, c, backButton)
        {
        }

        public override void Update()
        {
            string text = _textElement.text;
            int num = 0;
            foreach (MatchSetting matchSetting in TeamSelect2.matchSettings)
            {
                if (matchSetting.filtered)
                    ++num;
            }
            foreach (UnlockData unlock in Unlocks.GetUnlocks(UnlockType.Any))
            {
                if (unlock.enabled && unlock.filtered)
                    ++num;
            }
            if (num == 0)
                _textElement.text = "|DGBLUE|NO FILTERS";
            else
                _textElement.text = "|DGYELLOW|FILTERS: " + num.ToString();
            if (_textElement.text != text)
            {
                _textElement.Resize();
                _dirty = true;
                rightSection.Resize();
            }
            base.Update();
        }
    }
}
