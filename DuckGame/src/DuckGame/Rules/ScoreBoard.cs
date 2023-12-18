namespace DuckGame
{
    public class ScoreBoard : Thing
    {
        public ScoreBoard()
          : base()
        {
        }

        public override void Initialize()
        {
            int num = 0;
            foreach (Team team in Teams.all)
            {
                if (team.activeProfiles.Count > 0)
                {
                    Level.current.AddThing(new PlayerCard(num * 1f, new Vec2(-400f, 140 * num + 120), new Vec2(Graphics.width / 2 - 200, 140 * num + 120), team));
                    ++num;
                }
            }
        }
    }
}
