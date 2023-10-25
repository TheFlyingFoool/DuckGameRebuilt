namespace DuckGame
{
    public class NMClientLoadedLevel : NMDuckNetwork
    {
        public new byte levelIndex;

        public NMClientLoadedLevel()
        {
        }

        public NMClientLoadedLevel(byte idx) => levelIndex = idx;
    }
}
