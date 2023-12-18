namespace DuckGame
{
    [EditorGroup("Blocks")]
    [BaggedProperty("isInDemo", true)]
    public class BlueprintTileset : AutoBlock
    {
        public BlueprintTileset(float x, float y)
          : base(x, y, "blueprintTileset")
        {
            _editorName = "Blueprint";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 16f;
            verticalWidth = 14f;
            horizontalHeight = 16f;
        }
    }
}
