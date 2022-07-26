// Decompiled with JetBrains decompiler
// Type: DuckGame.NMSpecialHat
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMSpecialHat : NMDuckNetwork
    {
        private Team _team;
        private byte[] _data;
        public Profile profile;
        public ushort customTeamIndex;
        public bool filtered;

        public byte[] GetData() => this._data;

        public NMSpecialHat(Team pTeam, Profile pProfile, bool pFiltered)
        {
            this._team = pTeam;
            this.profile = pProfile;
            this.customTeamIndex = (ushort)Teams.core.extraTeams.IndexOf(pTeam);
            this.filtered = pFiltered;
        }

        public NMSpecialHat(Team pTeam, Profile pProfile)
        {
            this._team = pTeam;
            this.profile = pProfile;
            this.customTeamIndex = (ushort)Teams.core.extraTeams.IndexOf(pTeam);
        }

        public NMSpecialHat()
        {
        }

        protected override void OnSerialize()
        {
            if (this._team != null)
            {
                this.serializedData.Write(true);
                this.serializedData.Write(this.customTeamIndex);
                this.serializedData.Write(this.filtered ? (byte)1 : (byte)0);
                if (this.filtered)
                    return;
                this.serializedData.Write(new BitBuffer(this._team.customData), true);
            }
            else
                this.serializedData.Write(false);
        }

        public override void OnDeserialize(BitBuffer data)
        {
            if (!data.ReadBool())
                return;
            this.customTeamIndex = data.ReadUShort();
            this.filtered = data.ReadByte() == (byte)1;
            if (this.filtered)
                return;
            this._data = data.ReadBitBuffer().GetBytes();
        }
    }
}
