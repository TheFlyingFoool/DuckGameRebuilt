// Decompiled with JetBrains decompiler
// Type: DuckGame.CityTreeTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks")]
    [BaggedProperty("isInDemo", false)]
    public class CityTreeTileset : AutoPlatform
    {
        public CityTreeTileset(float x, float y)
          : base(x, y, "cityTree")
        {
            this._editorName = "City Tree";
            this.physicsMaterial = PhysicsMaterial.Wood;
            this.verticalWidth = 6f;
            this.verticalWidthThick = 15f;
            this.horizontalHeight = 8f;
            this._hasNubs = false;
            this.depth = -0.6f;
            this.placementLayerOverride = Layer.Blocks;
            this.treeLike = true;
        }
    }
}
