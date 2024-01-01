namespace DuckGame
{
    [EditorGroup("Blocks")]
    public class UndergroundTileset : AutoBlock
    {
        public UndergroundTileset(float x, float y)
          : base(x, y, "undergroundTileset")
        {
            _editorName = "Bunker";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 10f;
            verticalWidthThick = 15f;
            horizontalHeight = 15f;
            brokenSptiteIndex = 2;
        }

        public override void Draw() => base.Draw();
    }
}
