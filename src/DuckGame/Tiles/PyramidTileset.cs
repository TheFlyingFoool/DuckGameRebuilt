// Decompiled with JetBrains decompiler
// Type: DuckGame.PyramidTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks", EditorItemType.Pyramid)]
    [BaggedProperty("isInDemo", false)]
    public class PyramidTileset : AutoBlock
    {
        public PyramidTileset(float x, float y)
          : base(x, y, "pyramidTileset")
        {
            this._editorName = "Pyramid";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidthThick = 14f;
            this.verticalWidth = 12f;
            this.horizontalHeight = 13f;
        }
    }
}
