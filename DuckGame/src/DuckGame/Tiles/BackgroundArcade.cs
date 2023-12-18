namespace DuckGame
{
    [EditorGroup("Background")]
    public class BackgroundArcade : BackgroundTile
    {
        public BackgroundArcade(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("arcadeBackground", 16, 16, true);
            _opacityFromGraphic = true;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _editorName = "Arcade";
        }
    }
}
