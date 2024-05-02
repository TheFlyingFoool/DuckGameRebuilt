namespace DuckGame
{
    public class GhostConnectionData
    {
        private long _connectionStateMask = long.MaxValue;
        public ushort prevInputState;
        public uint latestCommandTickReceived;
        public NetIndex16 lastTickSent = (NetIndex16)1;
        public NetIndex16 lastTickReceived = (NetIndex16)0;
        public NetIndex8 authority = (NetIndex8)1;

        public long connectionStateMask
        {
            get => _connectionStateMask;
            set => _connectionStateMask = value;
        }
    }
}
