namespace DuckGame
{
    [FixedNetworkID(10)]
    public class NMHeartbeat : NMNetworkCoreMessage
    {
        public NetIndex4 remoteSession;

        public NMHeartbeat()
        {
        }

        public NMHeartbeat(NetIndex4 s) => remoteSession = s;
    }
}
