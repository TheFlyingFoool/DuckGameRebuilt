// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLevelData
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMLevelData : NMEvent
    {
        public new byte levelIndex;
        private Level _level;

        public NMLevelData() => manager = BelongsToManager.EventManager;

        public NMLevelData(Level pLevel)
        {
            manager = BelongsToManager.EventManager;
            levelIndex = pLevel.networkIndex = DuckNetwork.levelIndex;
            _level = pLevel;
        }

        public override void Activate()
        {
            if (DuckNetwork.levelIndex != levelIndex)
                return;
            DevConsole.Log(DCSection.DuckNet, "|DGGREEN|Received Level Information (" + levelIndex.ToString() + ").");
            Level.current.TransferComplete(connection);
            Send.Message(new NMLevelReady(levelIndex), NetMessagePriority.ReliableOrdered);
            connection.levelIndex = levelIndex;
        }
    }
}
