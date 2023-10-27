namespace DuckGame
{
    public class NMMakeNewConnection : NMNetworkCoreMessage
    {
        public string identifier;

        public NMMakeNewConnection()
        {
        }

        public NMMakeNewConnection(string pIdentifier) => identifier = pIdentifier;
    }
}
