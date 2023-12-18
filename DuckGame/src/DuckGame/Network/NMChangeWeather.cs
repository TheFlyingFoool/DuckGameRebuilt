namespace DuckGame
{
    public class NMChangeWeather : NMEvent
    {
        public byte weather;

        public NMChangeWeather()
        {
        }

        public NMChangeWeather(byte weatherVal) => weather = weatherVal;

        public override void Activate()
        {
            if (Level.current is RockScoreboard)
                (Level.current as RockScoreboard).SetWeather((Weather)weather);
            base.Activate();
        }
    }
}
