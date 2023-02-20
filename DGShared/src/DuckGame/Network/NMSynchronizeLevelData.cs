// Decompiled with JetBrains decompiler
// Type: DuckGame.NMSynchronizeLevelData
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
