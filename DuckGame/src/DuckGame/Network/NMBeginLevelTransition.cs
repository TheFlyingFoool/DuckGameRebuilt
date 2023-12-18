namespace DuckGame
{
    public class NMBeginLevelTransition : NMDuckNetworkEvent
    {
        public new byte levelIndex;

        public NMBeginLevelTransition()
        {
        }

        public NMBeginLevelTransition(byte pLevelIndex) => levelIndex = pLevelIndex;

        public override void Activate()
        {
        }
    }
}
