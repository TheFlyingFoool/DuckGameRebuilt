namespace DuckGame
{
    public class BlankDoor : Thing
    {
        private BitmapFont _fontSmall;

        public BlankDoor(float pX, float pY)
          : base(pX, pY, new Sprite("blank_door", Vec2.Zero))
        {
            _fontSmall = new BitmapFont("smallBiosFont", 7, 6);
        }

        public override void Draw()
        {
            _fontSmall.DrawOutline("DUCK", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, depth + 10);
            _fontSmall.DrawOutline("GAME", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, depth + 10);
            base.Draw();
        }
    }
}
