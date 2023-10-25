namespace DuckGame
{
    [FixedNetworkID(421)]
    public class NMBadSession : NMNetworkCoreMessage
    {
        public NetIndex4 remoteSession;

        public NMBadSession()
        {
        }

        public NMBadSession(NetIndex4 pMySession) => remoteSession = pMySession;
    }
}
