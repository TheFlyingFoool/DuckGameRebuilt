namespace DuckGame
{
    public class NMBeginLevel : NMSynchronizedEvent
    {
        public NMBeginLevel() => manager = BelongsToManager.EventManager;

        public override void Activate() => GameMode.getReady = true;
    }
}
