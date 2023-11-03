namespace DuckGame
{
    public class SwearingEvent : Event
    {
        public SwearingEvent(Profile dealerVal, Profile victimVal)
          : base(dealerVal, victimVal)
        {
            ++dealerVal.stats.timesSwore;
        }
    }
}
