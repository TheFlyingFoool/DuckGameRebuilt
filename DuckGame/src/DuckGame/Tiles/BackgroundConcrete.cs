namespace DuckGame
{
    [EditorGroup("Background")]
    public class BackgroundConcrete : BackgroundTile
    {
        public BackgroundConcrete(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("concrete", 16, 16);
            _opacityFromGraphic = true;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _editorName = "Concrete";
        }
    }
}
