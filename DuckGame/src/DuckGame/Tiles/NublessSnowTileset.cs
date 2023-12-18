namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    [BaggedProperty("isInDemo", false)]
    public class NublessSnowTileset : SnowTileset
    {
        public NublessSnowTileset(float x, float y)
          : base(x, y, "nublessSnow")
        {
            _editorName = "Snow NONUBS";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 15f;
            verticalWidth = 14f;
            horizontalHeight = 15f;
            _tileset = "snowTileset";
            _sprite = new SpriteMap("nublessSnow", 16, 16);
            graphic = _sprite;
            _sprite.frame = 40;
            willHeat = true;
            _impactThreshold = -1f;
            _hasNubs = false;
            meltedTileset = "nublessSnow";
            frozenTileset = "nublessIceTileset";
        }
    }
}
