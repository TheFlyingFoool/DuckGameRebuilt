namespace DuckGame
{
    public class NMClientNeedsLevelData : NMDuckNetwork
    {
        public new byte levelIndex;
        public ushort transferSession;

        public NMClientNeedsLevelData()
        {
        }

        public NMClientNeedsLevelData(byte idx, ushort tSession)
        {
            levelIndex = idx;
            transferSession = tSession;
        }
    }
}
