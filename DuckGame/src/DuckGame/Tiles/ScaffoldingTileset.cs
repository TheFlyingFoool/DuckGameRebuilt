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
            editorCycleType = typeof(WoodScaffoldingTileset);
        }
    }
}
