using System.Collections.Generic;

namespace DuckGame
{
    public class Pedestal : Thing
    {
        private Team _team;
        private SpriteMap _sprite;
        private Sprite _scoreCard;
        private BitmapFont _font;
        private Sprite _trophy;
        private List<Duck> _ducks = new List<Duck>();

        public Pedestal(float xpos, float ypos, Team team, int place, bool smallMode)
          : base(xpos, ypos)
        {
            _team = team;
            _sprite = !smallMode ? new SpriteMap("rockThrow/placePedastals", 38, 45) : new SpriteMap("rockThrow/placePedastalsSmall", 27, 45);
            _sprite.frame = place;
            center = new Vec2(_sprite.w / 2, _sprite.h);
            graphic = _sprite;
            depth = (Depth)0.062f;
            _scoreCard = new Sprite("rockThrow/scoreCard");
            _font = new BitmapFont("biosFont", 8);
            _scoreCard.CenterOrigin();
            _trophy = new Sprite("trophy");
            _trophy.CenterOrigin();
            if (Network.isServer)
            {
                int num1 = 0;
                foreach (Profile activeProfile in team.activeProfiles)
                {
                    float num2 = (team.activeProfiles.Count - 1) * 10;
                    Duck duck = new Duck(xpos - num2 / 2f + num1 * 10, GetYOffset() - 15f, activeProfile)
                    {
                        depth = (Depth)0.06f
                    };
                    Level.Add(duck);
                    if (place == 0)
                    {
                        Trophy trophy = new Trophy(duck.x, duck.y);
                        Level.Add(trophy);
                        if (!Network.isActive)
                        {
                            duck.Fondle(trophy);
                            duck.GiveHoldable(trophy);
                        }
                    }
                    ++num1;
                }
            }
            Level.Add(new Platform(xpos - 17f, GetYOffset(), 34f, 16f));
            Level.Add(new Block(-6f, GetYOffset() - 100f, 6f, 200f));
            Level.Add(new Block(320f, GetYOffset() - 100f, 6f, 200f));
            Level.Add(new Block(-20f, 155f, 600f, 100f));
        }

        public override void Update()
        {
        }

        public float GetYOffset()
        {
            float yoffset = y - 45f;
            if (_sprite.frame == 1)
                yoffset = y - 28f;
            else if (_sprite.frame == 2)
                yoffset = y - 19f;
            else if (_sprite.frame == 3)
                yoffset = y - 12f;
            return yoffset;
        }

        public override void Draw()
        {
            depth = -0.5f;
            base.Draw();
            int count = _team.activeProfiles.Count;
            if (_sprite.frame == 0)
            {
                _trophy.depth = depth + 1;
                Graphics.Draw(ref _trophy, x, y - 14f);
            }
            _scoreCard.depth = (Depth)1f;
            Graphics.Draw(ref _scoreCard, x, y + 2f);
            string text = Change.ToString(_team.score);
            _font.Draw(text, x - _font.GetWidth(text) / 2f, y, Color.DarkSlateGray, _scoreCard.depth + 1);
        }
    }
}
