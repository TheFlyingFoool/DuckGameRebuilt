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
