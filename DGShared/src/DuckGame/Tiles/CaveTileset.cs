// Decompiled with JetBrains decompiler
// Type: DuckGame.CaveTileset
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _editorName = "Cave";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 16f;
            verticalWidthThick = 16f;
            horizontalHeight = 16f;
            _hasNubs = false;
        }
    }
}
