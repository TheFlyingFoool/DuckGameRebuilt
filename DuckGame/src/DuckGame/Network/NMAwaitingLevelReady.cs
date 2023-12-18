namespace DuckGame
{
    public class NMAwaitingLevelReady : NMDuckNetwork
    {
        public new byte levelIndex;

        public NMAwaitingLevelReady()
        {
        }

        public NMAwaitingLevelReady(byte idx) => levelIndex = idx;
    }
}
