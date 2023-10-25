namespace DuckGame
{
    [EditorGroup("Blocks")]
    [BaggedProperty("isInDemo", true)]
    public class DonutTileset : AutoBlock
    {
        public DonutTileset(float x, float y)
          : base(x, y, "donutTileset")
        {
            _editorName = "Donut";
            physicsMaterial = PhysicsMaterial.Crust;
            verticalWidthThick = 15f;
            verticalWidth = 12f;
            horizontalHeight = 15f;
        }
    }
}
