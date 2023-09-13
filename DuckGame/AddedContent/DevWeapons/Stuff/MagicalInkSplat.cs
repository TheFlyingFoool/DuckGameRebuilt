namespace DuckGame
{
    public class MagicalInkSplat : Thing
    {
        public MagicalInkSplat(float x, float y)
            : base(x, y, new Sprite("title/editorBenchPaint"))
        {
            depth = 2f;
        }
    }
}