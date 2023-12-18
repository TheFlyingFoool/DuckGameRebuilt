namespace DuckGame
{
    [FixedNetworkID(43)]
    public class NMConnect : NMNetworkCoreMessage
    {
        public string version;
        public NetIndex4 connectsReceived;
        public NetIndex4 remoteSession;
        public string modHash;

        public NMConnect()
        {
        }

        public NMConnect(byte received, NetIndex4 s, string v, string mH)
        {
            version = v;
            connectsReceived = (NetIndex4)received;
            remoteSession = s;
            modHash = mH;
        }
    }
}
