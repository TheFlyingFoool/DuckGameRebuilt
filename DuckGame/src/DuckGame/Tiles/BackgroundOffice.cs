namespace DuckGame
{
    [EditorGroup("Background")]
    public class BackgroundOffice : BackgroundTile
    {
        public BackgroundOffice(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("officeBackground", 16, 16, true);
            _opacityFromGraphic = true;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _editorName = "Office";
        }
    }
}
