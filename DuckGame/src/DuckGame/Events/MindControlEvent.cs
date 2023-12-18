namespace DuckGame
{
    public class MindControlEvent : Event
    {
        public MindControlEvent(Profile dealerVal, Profile victimVal)
          : base(dealerVal, victimVal)
        {
        }
    }
}
