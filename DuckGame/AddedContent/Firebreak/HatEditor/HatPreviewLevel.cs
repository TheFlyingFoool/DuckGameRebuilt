namespace DuckGame
{
    public class HatPreviewLevel : Level
    {
        public HatPreviewLevel()
        {
            backgroundColor = Color.SlateGray;
        }

        public override void Initialize()
        {
            camera.position = camera.size / -2;
            
            Vec2 rectPos = camera.position + camera.size / 2;
            Rectangle bounds = new(rectPos - new Vec2(60, 24), rectPos + new Vec2(60, 24));
            SpawnInvisiblePrison(bounds);
            
            Add(new Duck(bounds.Center.x, bounds.Bottom, Profiles.DefaultPlayer1));
            
            base.Initialize();
        }

        public static void SpawnInvisiblePrison(Rectangle innerBounds)
        {
            Add(new Block(innerBounds.x, innerBounds.y - 16, innerBounds.width, 16));
            Add(new Block(innerBounds.x + innerBounds.width, innerBounds.y, 16, innerBounds.height));
            Add(new Block(innerBounds.x - 16, innerBounds.y, 16, innerBounds.height));
            Add(new Block(innerBounds.x, innerBounds.y + innerBounds.height, innerBounds.width, 16));
        }
    }
}