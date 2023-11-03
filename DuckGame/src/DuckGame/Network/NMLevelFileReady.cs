namespace DuckGame
{
    public class NMLevelFileReady : NMDuckNetwork
    {
        public new byte levelIndex;

        public NMLevelFileReady()
        {
        }

        public NMLevelFileReady(byte pLevelIndex) => levelIndex = pLevelIndex;
    }
}
