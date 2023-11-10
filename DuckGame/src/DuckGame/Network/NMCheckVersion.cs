namespace DuckGame
{
    [FixedNetworkID(41)]
    public class NMCheckVersion : NMNetworkCoreMessage
    {
        public string version;

        public NMCheckVersion()
        {
        }

        public NMCheckVersion(string v) => version = v;
    }
}
