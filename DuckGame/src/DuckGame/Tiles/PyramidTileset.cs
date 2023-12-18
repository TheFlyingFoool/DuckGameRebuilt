namespace DuckGame
{
    [EditorGroup("Blocks", EditorItemType.Pyramid)]
    [BaggedProperty("isInDemo", false)]
    public class PyramidTileset : AutoBlock
    {
        public PyramidTileset(float x, float y)
          : base(x, y, "pyramidTileset")
        {
            _editorName = "Pyramid";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 14f;
            verticalWidth = 12f;
            horizontalHeight = 13f;
        }
    }
}
