// Decompiled with JetBrains decompiler
// Type: DuckGame.NMSpecialHat
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public byte[] GetData() => _data;

        public NMSpecialHat(Team pTeam, Profile pProfile, bool pFiltered)
        {
            _team = pTeam;
            profile = pProfile;
            customTeamIndex = (ushort)Teams.core.extraTeams.IndexOf(pTeam);
            filtered = pFiltered;
        }

        public NMSpecialHat(Team pTeam, Profile pProfile)
        {
            _team = pTeam;
            profile = pProfile;
            customTeamIndex = (ushort)Teams.core.extraTeams.IndexOf(pTeam);
        }

        public NMSpecialHat()
        {
        }

        protected override void OnSerialize()
        {
            if (_team != null)
            {
                serializedData.Write(true);
                serializedData.Write(customTeamIndex);
                serializedData.Write(filtered ? (byte)1 : (byte)0);
                if (filtered)
                    return;
                serializedData.Write(new BitBuffer(_team.customData), true);
            }
            else
                serializedData.Write(false);
        }

        public override void OnDeserialize(BitBuffer data)
        {
            if (!data.ReadBool())
                return;
            customTeamIndex = data.ReadUShort();
            filtered = data.ReadByte() == 1;
            if (filtered)
                return;
            _data = data.ReadBitBuffer().GetBytes();
        }
    }
}
