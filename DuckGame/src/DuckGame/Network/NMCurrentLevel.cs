namespace DuckGame
{
    public class NMCurrentLevel : NMDuckNetwork
    {
        public new byte levelIndex;

        public NMCurrentLevel()
        {
        }

        public NMCurrentLevel(byte idx) => levelIndex = idx;

        public override string ToString() => base.ToString() + "(index = " + levelIndex.ToString() + ")";
    }
}
