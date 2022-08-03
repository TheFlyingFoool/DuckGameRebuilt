// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLevelReady
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
