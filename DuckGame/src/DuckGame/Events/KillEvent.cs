namespace DuckGame
{
    public class KillEvent : Event
    {
        private System.Type _weapon;

        public System.Type weapon => _weapon;

        public KillEvent(Profile killerVal, Profile killedVal, System.Type weapon)
          : base(killerVal, killedVal)
        {
            _weapon = weapon;
        }
    }
}
