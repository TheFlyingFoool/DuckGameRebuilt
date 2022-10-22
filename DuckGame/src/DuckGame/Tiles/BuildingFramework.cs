// Decompiled with JetBrains decompiler
// Type: DuckGame.BuildingFramework
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks")]
    public class BuildingFramework : AutoBlock
    {
        public BuildingFramework(float x, float y)
          : base(x, y, "buildingFrame")
        {
            _editorName = "Framework";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 10f;
            verticalWidthThick = 13f;
            horizontalHeight = 10f;
            _hasNubs = false;
            indestructable = true;
            layer = Layer.Blocks;
            depth = -0.5f;
            placementLayerOverride = Layer.Game;
        }

        public override void Draw() => base.Draw();
    }
}
