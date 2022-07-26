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

        public NMProfileNetData() => this.manager = BelongsToManager.GhostManager;

        public NMProfileNetData(Profile pProfile, NetworkConnection pConnection)
          : this()
        {
            this._profile = pProfile;
            this._connection = pConnection;
        }

        protected override void OnSerialize()
        {
            if (!DuckNetwork.profiles.Contains(this._profile))
                return;
            this._serializedData.WriteProfile(this._profile);
            BitBuffer val = this._profile.netData.Serialize(this._connection, this._hashes);
            this.syncIndex = this._profile.netData.GetSyncIndex(this._connection);
            this._serializedData.Write(val, true);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            this._profile = d.ReadProfile();
            this._netData = d.ReadBitBuffer();
        }
    }
}
