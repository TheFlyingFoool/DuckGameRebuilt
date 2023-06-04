// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextSearch
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ContextSearch : ContextMenu
    {
        //private bool _searching;

        public ContextSearch(IContextListener owner)
          : base(owner)
        {
            itemSize.y = 16f;
            _text = "@searchicon@ search...";
            tooltip = "Search for an object! (Shortcut: @COMMA@)";
        }

        public override void Selected() => (Level.current as Editor).searching = true;

        public override void Draw()
        {
            if (_hover && !greyOut)
                Graphics.DrawRect(position, position + itemSize, new Color(70, 70, 70), depth + 1);
            Color color = Color.White;
            if (greyOut)
                color = Color.White * 0.3f;
            if (hover)
                _text = "@searchiconwhite@ search...";
            else
                _text = "@searchicon@ |GRAY|search...";
            Graphics.DrawFancyString(_text, position + new Vec2(0f, 4f), color, depth + 2);
        }
    }
}
