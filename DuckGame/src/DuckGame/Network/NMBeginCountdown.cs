namespace DuckGame
{
    public class NMBeginCountdown : NMSynchronizedEvent
    {
        public NMBeginCountdown() => manager = BelongsToManager.EventManager;

        public override void Activate() => DuckNetwork.core.startCountdown = true;
    }
}
