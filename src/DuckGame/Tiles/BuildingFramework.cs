// Decompiled with JetBrains decompiler
// Type: DuckGame.BuildingFramework
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._editorName = "Framework";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidth = 10f;
            this.verticalWidthThick = 13f;
            this.horizontalHeight = 10f;
            this._hasNubs = false;
            this.indestructable = true;
            this.layer = Layer.Blocks;
            this.depth = - 0.5f;
            this.placementLayerOverride = Layer.Game;
        }

        public override void Draw() => base.Draw();
    }
}
