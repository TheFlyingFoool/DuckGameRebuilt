namespace DuckGame
{
    public class NMDuckNetworkSynchronized : SynchronizedNetMessage
    {
        public NMDuckNetworkSynchronized() => manager = BelongsToManager.DuckNetwork;
    }
}
