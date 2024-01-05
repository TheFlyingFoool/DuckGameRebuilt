namespace DuckGame
{
    [EditorGroup("Blocks")]
    public class CastleTileset : AutoBlock
    {
        public CastleTileset(float x, float y)
          : base(x, y, "castle")
        {
            _editorName = "Castle";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 10f;
            verticalWidthThick = 14f;
            horizontalHeight = 14f;
            _hasNubs = false;
            brokenSptiteIndex = 3;
        }
    }
}
