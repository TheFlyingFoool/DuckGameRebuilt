namespace DuckGame
{
    [FixedNetworkID(9)]
    public class NMLegacyConnect : NMNetworkCoreMessage
    {
        public byte connectsReceived;
        public NetIndex4 remoteSession;

        public NMLegacyConnect()
        {
        }

        public NMLegacyConnect(byte received, NetIndex4 s)
        {
            connectsReceived = received;
            remoteSession = s;
        }
    }
}
