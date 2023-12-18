namespace DuckGame
{
    public class Snare : Drum
    {
        private Sprite _stand;

        public Snare(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("drumset/snareDrum");
            center = new Vec2(graphic.w / 2, graphic.h / 2);
            _stand = new Sprite("drumset/snareStand");
            _stand.center = new Vec2(_stand.w / 2, 0f);
            _sound = "snare";
        }

        public override void Draw()
        {
            base.Draw();
            _stand.depth = depth - 1;
            Graphics.Draw(ref _stand, x, y + 3f);
        }
    }
}
