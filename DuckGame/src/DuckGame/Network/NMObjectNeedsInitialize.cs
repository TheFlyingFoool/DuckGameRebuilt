namespace DuckGame
{
    public class NMObjectNeedsInitialize : NMDuckNetworkEvent
    {
        public Thing thing;

        public NMObjectNeedsInitialize(Thing t) => thing = t;

        public NMObjectNeedsInitialize()
        {
        }

        public override void Activate()
        {
            if (thing == null || thing.ghostObject == null)
                return;
            if (thing.connection == connection)
                Thing.Fondle(thing, DuckNetwork.localConnection);
            if (!thing.isServerForObject)
                return;
            thing.ghostObject.DirtyStateMask(long.MaxValue, connection);
        }
    }
}
