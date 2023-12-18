namespace DuckGame
{
    [EditorGroup("Blocks")]
    [BaggedProperty("previewPriority", true)]
    public class IndustrialTileset : AutoBlock
    {
        public IndustrialTileset(float x, float y)
          : base(x, y, "industrialTileset")
        {
            _editorName = "Industrial";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 14f;
            verticalWidthThick = 15f;
            horizontalHeight = 15f;
        }

        public override void Draw() => base.Draw();
    }
}
