namespace DuckGame
{
    public class DartHitEvent : Event
    {
        public DartHitEvent(Profile dealerVal, Profile victimVal)
          : base(dealerVal, victimVal)
        {
        }
    }
}
