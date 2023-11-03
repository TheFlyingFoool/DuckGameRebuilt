namespace DuckGame
{
    public class NMAllClientsReady : NMSynchronizedEvent
    {
        public NMAllClientsReady() => manager = BelongsToManager.EventManager;

        public override void Activate() => Level.current.DoAllClientsReady();
    }
}
