namespace DuckGame
{
    public class CrashCymbal : Drum
    {
        private Sprite _stand;

        public CrashCymbal(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("drumset/crashCymbal");
            center = new Vec2(graphic.w / 2, graphic.h / 2);
            _stand = new Sprite("drumset/crashStand");
            _stand.center = new Vec2(_stand.w / 2, 0f);
            _sound = "crash";
        }

        public override void Draw()
        {
            base.Draw();
            _stand.depth = depth - 1;
            Graphics.Draw(ref _stand, x, y + 1f);
        }
    }
}
