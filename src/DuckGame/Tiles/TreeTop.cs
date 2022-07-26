// Decompiled with JetBrains decompiler
// Type: DuckGame.TreeTop
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class TreeTop : Thing
    {
        private Sprite _treeInside;

        public TreeTop(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("treeTop");
            this._treeInside = new Sprite("treeTopInside");
            this._treeInside.center = new Vec2(24f, 24f);
            this._treeInside.alpha = 0.8f;
            this._treeInside.depth = (Depth)0.9f;
            this.center = new Vec2(24f, 24f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Left | WallHug.Right | WallHug.Ceiling | WallHug.Floor;
        }

        public override void Draw()
        {
            this.graphic.flipH = this.offDir <= (sbyte)0;
            base.Draw();
        }
    }
}
