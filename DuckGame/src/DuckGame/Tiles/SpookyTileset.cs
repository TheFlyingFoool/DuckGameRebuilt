namespace DuckGame
{
    [EditorGroup("Blocks")]
    public class SpookyTileset : AutoBlock
    {
        public SpookyTileset(float x, float y)
          : base(x, y, "spookyTileset")
        {
            _editorName = "Spooky";
            physicsMaterial = PhysicsMaterial.Wood;
            verticalWidth = 10f;
            verticalWidthThick = 15f;
            horizontalHeight = 14f;
            brokenSptiteIndex = 13;
        }

        public override void Draw() => base.Draw();
    }
}
