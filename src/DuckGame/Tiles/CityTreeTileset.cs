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
            _editorName = "City Tree";
            physicsMaterial = PhysicsMaterial.Wood;
            verticalWidth = 6f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            _hasNubs = false;
            depth = -0.6f;
            placementLayerOverride = Layer.Blocks;
            treeLike = true;
        }
    }
}
