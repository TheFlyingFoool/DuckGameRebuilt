namespace DuckGame
{
    public class NMSynchronizeLevelData : NMDuckNetworkEvent
    {
        public override void Activate()
        {
            string guid = Deathmatch.RandomLevelString("", "deathmatch", true);
            if (!(guid == ""))
                return;
            LevelData level = Content.GetLevel(guid);
            int checksum = (int)level.GetChecksum();
            XMLLevel.GetCompressedLevelData(level, guid.Substring(0, guid.Length - 7));
        }
    }
}
