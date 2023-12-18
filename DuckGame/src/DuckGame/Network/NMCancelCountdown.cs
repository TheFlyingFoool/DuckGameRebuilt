namespace DuckGame
{
    public class NMCancelCountdown : NMSynchronizedEvent
    {
        public NMCancelCountdown() => manager = BelongsToManager.EventManager;

        public override void Activate() => DuckNetwork.core.startCountdown = false;
    }
}
