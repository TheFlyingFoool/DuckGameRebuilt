// Decompiled with JetBrains decompiler
// Type: DuckGame.PineTreeTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    public class PineTreeTileset : PineTree
    {
        public PineTreeTileset(float x, float y)
          : base(x, y, "pineTileset")
        {
            this._editorName = "Pine";
            this.physicsMaterial = PhysicsMaterial.Default;
            this.verticalWidth = 14f;
            this.verticalWidthThick = 15f;
            this.horizontalHeight = 8f;
            this.depth = - 0.55f;
        }
    }
}
