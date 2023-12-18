namespace DuckGame
{
    [EditorGroup("Blocks")]
    public class CityTileset : AutoBlock
    {
        public CityTileset(float x, float y)
          : base(x, y, "cityTileset")
        {
            _editorName = "City";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 10f;
            verticalWidthThick = 15f;
            horizontalHeight = 13f;
        }

        public override void Draw() => base.Draw();
    }
}
