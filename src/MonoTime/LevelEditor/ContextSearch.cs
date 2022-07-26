// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextSearch
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ContextSearch : ContextMenu
    {
        private bool _searching;

        public ContextSearch(IContextListener owner)
          : base(owner)
        {
            this.itemSize.y = 16f;
            this._text = "@searchicon@ search...";
            this.tooltip = "Search for an object!";
        }

        public override void Selected() => (Level.current as Editor).searching = true;

        public override void Draw()
        {
            if (this._hover && !this.greyOut)
                Graphics.DrawRect(this.position, this.position + this.itemSize, new Color(70, 70, 70), this.depth + 1);
            Color color = Color.White;
            if (this.greyOut)
                color = Color.White * 0.3f;
            if (this.hover)
                this._text = "@searchiconwhite@ search...";
            else
                this._text = "@searchicon@ |GRAY|search...";
            Graphics.DrawFancyString(this._text, this.position + new Vec2(0.0f, 4f), color, this.depth + 2);
        }
    }
}
