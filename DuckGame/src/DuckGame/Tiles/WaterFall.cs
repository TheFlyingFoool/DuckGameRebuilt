namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class WaterFall : Thing
    {
        public WaterFall(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("waterFall", 16, 32);
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(8f, 8f);
            _collisionOffset = new Vec2(-8f, -8f);
            layer = Layer.Foreground;
            depth = (Depth)0.9f;
            alpha = 0.8f;
        }

        public override void Draw()
        {
            (graphic as SpriteMap).frame = (int)(Graphics.frame / 3 % 4);
            graphic.flipH = offDir <= 0;
            base.Draw();
        }
    }
}
