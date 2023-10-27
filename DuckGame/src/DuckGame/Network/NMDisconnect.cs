namespace DuckGame
{
    [FixedNetworkID(8)]
    public class NMDisconnect : NMNetworkCoreMessage
    {
        public byte error;

        public DuckNetErrorInfo GetError() => new DuckNetErrorInfo((DuckNetError)error, ((DuckNetError)error).ToString());

        public NMDisconnect(DuckNetError pError) => error = (byte)pError;

        public NMDisconnect(byte pError) => error = pError;

        public NMDisconnect()
        {
        }
    }
}
