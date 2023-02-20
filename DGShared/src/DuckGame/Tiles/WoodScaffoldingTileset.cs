// Decompiled with JetBrains decompiler
// Type: DuckGame.WoodScaffoldingTileset
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|Jump Through")]
    public class WoodScaffoldingTileset : AutoPlatform
    {
        public WoodScaffoldingTileset(float x, float y)
          : base(x, y, "woodScaffolding")
        {
            _editorName = "Scaffold (Wood)";
            physicsMaterial = PhysicsMaterial.Default;
            verticalWidth = 14f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            _hasNubs = false;
            _collideBottom = true;
        }
    }
}
