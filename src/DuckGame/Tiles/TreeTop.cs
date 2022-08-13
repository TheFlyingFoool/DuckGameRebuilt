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
            graphic = new Sprite("treeTop");
            _treeInside = new Sprite("treeTopInside")
            {
                center = new Vec2(24f, 24f),
                alpha = 0.8f,
                depth = (Depth)0.9f
            };
            center = new Vec2(24f, 24f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Left | WallHug.Right | WallHug.Ceiling | WallHug.Floor;
            shouldbeinupdateloop = false;
        }

        public override void Draw()
        {
            graphic.flipH = offDir <= 0;
            base.Draw();
        }
    }
}
