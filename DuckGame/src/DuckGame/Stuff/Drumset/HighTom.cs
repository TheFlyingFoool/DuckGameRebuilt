namespace DuckGame
{
    public class HighTom : Drum
    {
        public HighTom(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("drumset/smallTom");
            center = new Vec2(graphic.w / 2, graphic.h / 2);
            _sound = "hiTom";
        }

        public override void Draw() => base.Draw();
    }
}
