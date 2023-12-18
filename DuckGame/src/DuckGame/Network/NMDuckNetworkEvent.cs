namespace DuckGame
{
    public class NMDuckNetworkEvent : NMDuckNetwork
    {
        public NMDuckNetworkEvent() => manager = BelongsToManager.DuckNetwork;

        public virtual void Activate()
        {
        }
    }
}
