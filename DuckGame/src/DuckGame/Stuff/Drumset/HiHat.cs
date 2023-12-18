namespace DuckGame
{
    public class HiHat : Drum
    {
        private Sprite _stand;

        public HiHat(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("drumset/hat");
            center = new Vec2(graphic.w / 2, graphic.h / 2);
            _stand = new Sprite("drumset/hatStand");
            _stand.center = new Vec2(_stand.w / 2, 0f);
            _sound = "hatClosed";
            _alternateSound = "hatOpen";
        }

        public override void Draw()
        {
            base.Draw();
            _stand.depth = depth - 1;
            Graphics.Draw(ref _stand, x, y - 4f);
        }
    }
}
