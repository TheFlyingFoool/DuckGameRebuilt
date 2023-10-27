namespace DuckGame
{
    public class BassDrum : Drum
    {
        public BassDrum(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("drumset/bassDrum");
            center = new Vec2(graphic.w / 2, graphic.h / 2);
            _sound = "kick";
        }
    }
}
