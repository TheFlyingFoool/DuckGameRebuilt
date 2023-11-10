namespace DuckGame
{
    public class NMEvent : NetMessage
    {
        public NMEvent() => manager = BelongsToManager.EventManager;

        public virtual void Activate()
        {
        }
    }
}
