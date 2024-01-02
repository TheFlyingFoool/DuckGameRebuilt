namespace DuckGame
{
    [EditorGroup("Blocks")]
    [BaggedProperty("previewPriority", true)]
    public class SpaceTileset : AutoBlock
    {
        public SpaceTileset(float x, float y)
          : base(x, y, "spaceTileset")
        {
            _editorName = "Space";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 10f;
            verticalWidthThick = 15f;
            horizontalHeight = 15f;
            brokenSptiteIndex = 12;
        }
    }
}
