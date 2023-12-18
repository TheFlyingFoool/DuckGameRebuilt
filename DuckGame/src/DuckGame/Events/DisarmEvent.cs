namespace DuckGame
{
    public class DisarmEvent : Event
    {
        public DisarmEvent(Profile dealerVal, Profile victimVal)
          : base(dealerVal, victimVal)
        {
            if (dealerVal != null)
                ++dealer.stats.disarms;
            if (victimVal == null)
                return;
            ++victimVal.stats.timesDisarmed;
        }
    }
}
