using DuckGame;
using System;
using System.Collections.Generic;
using System.Windows.Automation.Peers;
namespace AddedContent.Biverom.TreeSawing
{
    public class TreeEnd : Thing
    {
        public TreeEnd(float xpos, float ypos, bool isCity, bool explode)
            : base(xpos, ypos)
        {
            this.graphic = new SpriteMap(isCity ? "cityTreeStump" : "treeStump", 16, 16);
            (this.graphic as SpriteMap).frame = (explode ? 2 : 0) + 1;
            this._center = new Vec2(8f, 8f);
        }
    }
}
