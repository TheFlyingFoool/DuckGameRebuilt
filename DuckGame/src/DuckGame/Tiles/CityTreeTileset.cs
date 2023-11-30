namespace DuckGame
{
    [EditorGroup("Blocks")]
    [BaggedProperty("isInDemo", false)]
    public class CityTreeTileset : AutoPlatform
    {
        public CityTreeTileset(float x, float y)
          : base(x, y, "cityTree")
        {
            _editorName = "City Tree";
            physicsMaterial = PhysicsMaterial.Wood;
            verticalWidth = 6f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            _hasNubs = false;
            depth = -0.6f;
            placementLayerOverride = Layer.Blocks;
            treeLike = true;
        }
    }
}
