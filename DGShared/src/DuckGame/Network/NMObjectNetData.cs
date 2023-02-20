// Decompiled with JetBrains decompiler
// Type: DuckGame.NMObjectNetData
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public NMObjectNetData() => manager = BelongsToManager.GhostManager;

        public NMObjectNetData(Thing pThing, NetworkConnection pConnection)
          : this()
        {
            thing = pThing;
            _connection = pConnection;
        }

        protected override void OnSerialize()
        {
            _serializedData.Write(thing);
            _serializedData.Write((object)thing.authority);
            BitBuffer val = thing._netData.Serialize(_connection, _hashes);
            syncIndex = thing._netData.GetSyncIndex(_connection);
            _serializedData.Write(val, true);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            thing = d.Read<Thing>();
            authority = (NetIndex8)d.ReadByte();
            _netData = d.ReadBitBuffer();
        }
    }
}
