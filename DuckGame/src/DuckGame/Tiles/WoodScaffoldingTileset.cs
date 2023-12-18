namespace DuckGame
{
    [EditorGroup("Blocks|Jump Through")]
    public class WoodScaffoldingTileset : AutoPlatform
    {
        public WoodScaffoldingTileset(float x, float y)
          : base(x, y, "woodScaffolding")
        {
            _editorName = "Scaffold (Wood)";
            physicsMaterial = PhysicsMaterial.Default;
            verticalWidth = 14f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            _hasNubs = false;
            _collideBottom = true;
            editorCycleType = typeof(TreeTileset);
        }
    }
}
