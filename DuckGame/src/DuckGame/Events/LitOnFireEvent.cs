namespace DuckGame
{
    public class LitOnFireEvent : Event
    {
        public LitOnFireEvent(Profile dealerVal, Profile victimVal)
          : base(dealerVal, victimVal)
        {
        }
    }
}
