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
            brokenSptiteIndex = 14;
        }
    }
}
