namespace DuckGame
{
    public class NMRequiresNewConnection : NMDuckNetwork
    {
        public string toWhom;

        public NMRequiresNewConnection()
        {
        }

        public NMRequiresNewConnection(string who) => toWhom = who;
    }
}
