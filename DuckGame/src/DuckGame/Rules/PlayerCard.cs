using System.Collections.Generic;

namespace DuckGame
{
    public class PlayerCard : Thing
    {
        private float _slideWait;
        private Vec2 _start;
        private Vec2 _end;
        private List<SpriteMap> _sprites = new List<SpriteMap>();
        private Team _team;

        public PlayerCard(float slideWait, Vec2 start, Vec2 end, Team team)
          : base()
        {
            layer = Layer.HUD;
            _start = start;
            _end = end;
            _slideWait = slideWait;
            position = _start;
            _team = team;
        }

        public override void Draw()
        {
        }
    }
}
