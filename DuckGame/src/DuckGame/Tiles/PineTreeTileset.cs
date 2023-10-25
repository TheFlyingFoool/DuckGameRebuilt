namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    public class PineTreeTileset : PineTree
    {
        public PineTreeTileset(float x, float y)
          : base(x, y, "pineTileset")
        {
            _editorName = "Pine";
            physicsMaterial = PhysicsMaterial.Default;
            verticalWidth = 14f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            depth = -0.55f;
        }
    }
}
