// Decompiled with JetBrains decompiler
// Type: DuckGame.ScaffoldingTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|Jump Through")]
    [BaggedProperty("previewPriority", true)]
    public class ScaffoldingTileset : AutoPlatform
    {
        public ScaffoldingTileset(float x, float y)
          : base(x, y, "scaffolding")
        {
            _editorName = "Scaffold";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 14f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            _collideBottom = true;
        }
    }
}
