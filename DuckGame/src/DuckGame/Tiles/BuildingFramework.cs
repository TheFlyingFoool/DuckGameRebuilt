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
