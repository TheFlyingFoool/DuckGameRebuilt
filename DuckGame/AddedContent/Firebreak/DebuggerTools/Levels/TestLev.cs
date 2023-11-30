namespace DuckGame
{
    public class TestLev : Level
    {
        public override void Initialize()
        {
            // backgroundColor = Color.SlateBlue;
        }

        public override void Draw()
        {
            for (int i = 0; i < DGRDevs.All.Length; i++)
            {
                DGRebuiltDeveloper dev = DGRDevs.All[i];

                Rectangle rectangle = new Rectangle(i * 16, 0, 16, 16);
                Graphics.DrawRect(rectangle, dev.Color, 1f);
                Graphics.DrawString(string.Join("\n", dev.DisplayName.ToCharArray()), new Vec2(rectangle.x + 4, rectangle.y + rectangle.height + 2), dev.Color);
            }
            
            base.Draw();
        }
    }
}