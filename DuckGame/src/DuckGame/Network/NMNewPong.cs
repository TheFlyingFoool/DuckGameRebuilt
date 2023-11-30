namespace DuckGame
{
    [FixedNetworkID(30015)]
    public class NMNewPong : NMNetworkCoreMessage
    {
        public byte index;

        public NMNewPong(byte pIndex) => index = pIndex;

        public NMNewPong()
        {
        }
    }
}
