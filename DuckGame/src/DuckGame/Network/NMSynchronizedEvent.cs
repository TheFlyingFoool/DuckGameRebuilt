namespace DuckGame
{
    public class NMSynchronizedEvent : SynchronizedNetMessage
    {
        public NMSynchronizedEvent() => manager = BelongsToManager.EventManager;

        public virtual void Activate()
        {
        }
    }
}
