// Decompiled with JetBrains decompiler
// Type: DuckGame.CaveTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks")]
    public class CaveTileset : AutoBlock
    {
        public CaveTileset(float x, float y)
          : base(x, y, "caveTileset")
        {
            this._editorName = "Cave";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidth = 16f;
            this.verticalWidthThick = 16f;
            this.horizontalHeight = 16f;
            this._hasNubs = false;
        }
    }
}
