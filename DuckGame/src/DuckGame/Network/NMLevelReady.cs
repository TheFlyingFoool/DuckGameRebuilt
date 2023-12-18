namespace DuckGame
{
    public class NMLevelReady : NMDuckNetworkEvent
    {
        public new byte levelIndex;

        public NMLevelReady()
        {
        }

        public NMLevelReady(byte pLevelIndex) => levelIndex = pLevelIndex;

        public override void Activate()
        {
            DevConsole.Log(DCSection.DuckNet, "|DGORANGE|Level ready message(" + connection.levelIndex.ToString() + " -> " + levelIndex.ToString() + ")", connection);
            connection.levelIndex = levelIndex;
            if (!Network.isServer || levelIndex != DuckNetwork.levelIndex)
                return;
            Level.current.ClientReady(connection);
        }
    }
}
