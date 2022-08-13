// Decompiled with JetBrains decompiler
// Type: DuckGame.BlueprintTileset
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks")]
    [BaggedProperty("isInDemo", true)]
    public class BlueprintTileset : AutoBlock
    {
        public BlueprintTileset(float x, float y)
          : base(x, y, "blueprintTileset")
        {
            _editorName = "Blueprint";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 16f;
            verticalWidth = 14f;
            horizontalHeight = 16f;
        }
    }
}
