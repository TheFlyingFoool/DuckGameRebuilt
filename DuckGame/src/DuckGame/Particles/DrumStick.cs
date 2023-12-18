namespace DuckGame
{
    public class DrumStick : Thing
    {
        private float _startY;

        public DrumStick(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("drumset/drumStick");
            center = new Vec2(graphic.w / 2, graphic.h / 2);
            _startY = ypos;
            vSpeed = -3f;
        }

        public override void Update()
        {
            angle += 0.6f;
            y += vSpeed;
            vSpeed += 0.2f;
            if (y <= _startY)
                return;
            Level.Remove(this);
        }
    }
}
