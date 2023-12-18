namespace DuckGame
{
    public class SpriteThing : Thing
    {
        public DuckPersona persona;
        public Color color;

        public SpriteThing(float xpos, float ypos, Sprite spr)
          : base(xpos, ypos, spr)
        {
            //shouldbegraphicculled = false;
            collisionSize = new Vec2(spr.width, spr.height);
            center = new Vec2(spr.w / 2, spr.h / 2);
            collisionOffset = new Vec2(-(spr.w / 2), -(spr.h / 2));
            color = Color.White;
        }

        public override void Draw()
        {
            graphic.flipH = flipHorizontal;
            graphic.color = color;
            base.Draw();
        }
    }
}
