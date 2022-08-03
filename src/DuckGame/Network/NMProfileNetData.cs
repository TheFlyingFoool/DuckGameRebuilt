// Decompiled with JetBrains decompiler
// Type: DuckGame.NMProfileNetData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class NMProfileNetData : NetMessage
    {
        public NetIndex16 syncIndex;
        public HashSet<int> _hashes = new HashSet<int>();
        public Profile _profile;
        private NetworkConnection _connection;
        public BitBuffer _netData;

        public NMProfileNetData() => manager = BelongsToManager.GhostManager;

        public NMProfileNetData(Profile pProfile, NetworkConnection pConnection)
          : this()
        {
            _profile = pProfile;
            _connection = pConnection;
        }

        protected override void OnSerialize()
        {
            if (!DuckNetwork.profiles.Contains(_profile))
                return;
            _serializedData.WriteProfile(_profile);
            BitBuffer val = _profile.netData.Serialize(_connection, _hashes);
            syncIndex = _profile.netData.GetSyncIndex(_connection);
            _serializedData.Write(val, true);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            _profile = d.ReadProfile();
            _netData = d.ReadBitBuffer();
        }
    }
}
