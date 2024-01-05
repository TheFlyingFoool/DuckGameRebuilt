namespace DuckGame
{
    [EditorGroup("Blocks")]
    [BaggedProperty("previewPriority", true)]
    public class OfficeTileset : AutoBlock
    {
        public OfficeTileset(float x, float y)
          : base(x, y, "officeTileset2")
        {
            _editorName = "Office";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 10f;
            verticalWidthThick = 15f;
            horizontalHeight = 15f;
            brokenSptiteIndex = 10;
        }

        public override void Draw() => base.Draw();
    }
}
