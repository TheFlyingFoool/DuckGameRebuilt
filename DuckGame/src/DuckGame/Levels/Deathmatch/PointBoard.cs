namespace DuckGame
{
    public class PointBoard : Thing
    {
        private BitmapFont _font;
        private Sprite _scoreCard;
        private Team _team;
        private Thing _stick;

        public PointBoard(Thing rock, Team t)
          : base(rock.x + 24f, rock.y)
        {
            _scoreCard = new Sprite("rockThrow/scoreCard");
            _font = new BitmapFont("biosFont", 8);
            _team = t;
            _scoreCard.CenterOrigin();
            collisionOffset = new Vec2(-8f, -6f);
            collisionSize = new Vec2(16f, 13f);
            center = new Vec2(_scoreCard.w / 2, _scoreCard.h / 2);
            _stick = rock;
            depth = -0.1f;
        }

        public override void Update()
        {
            x = _stick.x + 24f;
            y = _stick.y;
        }

        public override void Draw()
        {
            _scoreCard.depth = depth;
            Graphics.Draw(ref _scoreCard, x, y);
            if (_team == null)
            {
                string text = "X";
                _font.Draw(text, x - _font.GetWidth(text) / 2f, y - 2f, Color.DarkSlateGray, _scoreCard.depth + 1);
            }
            else
            {
                string text = Change.ToString(_team.score);
                _font.Draw(text, x - _font.GetWidth(text) / 2f, y - 2f, Color.DarkSlateGray, _scoreCard.depth + 1);
            }
        }
    }
}
