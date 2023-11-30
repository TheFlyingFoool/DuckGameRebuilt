namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class CityRamp : IceWedge
    {
        public CityRamp(float xpos, float ypos, int dir)
          : base(xpos, ypos, dir)
        {
            _canFlipVert = true;
            graphic = new SpriteMap("cityWedge", 17, 17);
            hugWalls = WallHug.Left | WallHug.Right | WallHug.Floor;
            center = new Vec2(8f, 14f);
            collisionSize = new Vec2(14f, 8f);
            collisionOffset = new Vec2(-7f, -6f);
            _editorName = "Ramp";
            depth = -0.9f;
        }
    }
}
