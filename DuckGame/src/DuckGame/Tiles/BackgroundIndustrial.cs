namespace DuckGame
{
    [EditorGroup("Background")]
    public class BackgroundIndustrial : BackgroundTile
    {
        public BackgroundIndustrial(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("industrialBackground", 16, 16);
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _editorName = "Industrial";
        }
    }
}
