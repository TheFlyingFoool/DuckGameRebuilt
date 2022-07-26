// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLevelData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMLevelData : NMEvent
    {
        public new byte levelIndex;
        private Level _level;

        public NMLevelData() => this.manager = BelongsToManager.EventManager;

        public NMLevelData(Level pLevel)
        {
            this.manager = BelongsToManager.EventManager;
            this.levelIndex = pLevel.networkIndex = DuckNetwork.levelIndex;
            this._level = pLevel;
        }

        public override void Activate()
        {
            if ((int)DuckNetwork.levelIndex != (int)this.levelIndex)
                return;
            DevConsole.Log(DCSection.DuckNet, "|DGGREEN|Received Level Information (" + this.levelIndex.ToString() + ").");
            Level.current.TransferComplete(this.connection);
            Send.Message((NetMessage)new NMLevelReady(this.levelIndex), NetMessagePriority.ReliableOrdered);
            this.connection.levelIndex = this.levelIndex;
        }
    }
}
