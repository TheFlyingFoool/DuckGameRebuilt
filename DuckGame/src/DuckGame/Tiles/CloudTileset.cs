namespace DuckGame
{
    [EditorGroup("Blocks")]
    public class CloudTileset : AutoBlock
    {
        public CloudTileset(float x, float y)
          : base(x, y, "cloudTileset")
        {
            _editorName = "Kingdom";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 12f;
            verticalWidthThick = 14f;
            horizontalHeight = 13f;
            brokenSptiteIndex = 8;
        }

        public override void Draw() => base.Draw();
    }
}
