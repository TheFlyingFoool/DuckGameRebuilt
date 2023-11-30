namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class WaterFallTile : Thing
    {
        public WaterFallTile(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("waterFallTile", 16, 16);
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            layer = Layer.Foreground;
            depth = (Depth)0.9f;
            alpha = 0.8f;
            shouldbeinupdateloop = false;
        }

        public override void Draw()
        {
            (graphic as SpriteMap).frame = (int)(Graphics.frame / 3 % 4);
            graphic.flipH = offDir <= 0;
            base.Draw();
        }
    }
}
