namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    [BaggedProperty("isInDemo", false)]
    public class NublessSnowIceTileset : SnowIceTileset
    {
        public NublessSnowIceTileset(float x, float y)
          : base(x, y, "nublessIceTileset")
        {
            _editorName = "Snow Ice NONUBS";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 15f;
            verticalWidth = 14f;
            horizontalHeight = 15f;
            _impactThreshold = -1f;
            willHeat = true;
            _tileset = "snowTileset";
            _sprite = new SpriteMap("nublessIceTileset", 16, 16)
            {
                frame = 40
            };
            graphic = _sprite;
            _hasNubs = false;
            meltedTileset = "nublessSnow";
            frozenTileset = "nublessIceTileset";
            brokenSptiteIndex = 14;
        }
    }
}
