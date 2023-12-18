namespace DuckGame
{
    [FixedNetworkID(30001)]
    public class NMWrongVersion : NMNetworkCoreMessage
    {
        public string version;

        public NMWrongVersion()
        {
        }

        public NMWrongVersion(string v) => version = v;
    }
}
