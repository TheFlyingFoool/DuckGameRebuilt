namespace DuckGame
{
    public class VideoEditor
    {
        public static void Draw() => Graphics.DrawLine(new Vec2(32f, Layer.HUD.camera.height - 16f), new Vec2(Layer.HUD.camera.width - 32f, Layer.HUD.camera.height - 16f), Color.White, depth: ((Depth)1f));
    }
}
