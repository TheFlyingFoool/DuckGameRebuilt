namespace DuckGame
{
    [EditorGroup("Background")]
    [BaggedProperty("isInDemo", true)]
    public class ForegroundNature : ForegroundTile
    {
        public ForegroundNature(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("foregroundNature", 16, 16);
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            layer = Layer.Foreground;
            _editorName = "Foliage";
        }
    }
}
