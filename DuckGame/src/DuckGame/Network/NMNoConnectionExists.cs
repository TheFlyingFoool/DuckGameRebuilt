namespace DuckGame
{
    public class NMNoConnectionExists : NMDuckNetwork
    {
        public string toWhom;

        public NMNoConnectionExists()
        {
        }

        public NMNoConnectionExists(string who) => toWhom = who;
    }
}
