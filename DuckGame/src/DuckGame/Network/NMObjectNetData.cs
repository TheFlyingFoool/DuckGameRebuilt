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
