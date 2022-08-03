// Decompiled with JetBrains decompiler
// Type: DuckGame.NatureTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks")]
    [BaggedProperty("isInDemo", true)]
    public class NatureTileset : AutoBlock
    {
        public NatureTileset(float x, float y)
          : base(x, y, "natureTileset")
        {
            _editorName = "Nature";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 15f;
            verticalWidth = 14f;
            horizontalHeight = 15f;
        }
    }
}
