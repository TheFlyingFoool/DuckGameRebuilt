// Decompiled with JetBrains decompiler
// Type: DuckGame.NMObjectNetData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class NMObjectNetData : NetMessage
    {
        public NetIndex8 authority;
        public NetIndex16 syncIndex;
        public HashSet<int> _hashes = new HashSet<int>();
        public Thing thing;
        private NetworkConnection _connection;
        public BitBuffer _netData;
        public new byte levelIndex;

        public NMObjectNetData() => this.manager = BelongsToManager.GhostManager;

        public NMObjectNetData(Thing pThing, NetworkConnection pConnection)
          : this()
        {
            this.thing = pThing;
            this._connection = pConnection;
        }

        protected override void OnSerialize()
        {
            this._serializedData.Write((object)this.thing);
            this._serializedData.Write((object)this.thing.authority);
            BitBuffer val = this.thing._netData.Serialize(this._connection, this._hashes);
            this.syncIndex = this.thing._netData.GetSyncIndex(this._connection);
            this._serializedData.Write(val, true);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            this.thing = d.Read<Thing>();
            this.authority = (NetIndex8)(int)d.ReadByte();
            this._netData = d.ReadBitBuffer();
        }
    }
}
