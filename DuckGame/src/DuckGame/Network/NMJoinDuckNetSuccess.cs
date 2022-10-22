// Decompiled with JetBrains decompiler
// Type: DuckGame.NMJoinDuckNetSuccess
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class NMJoinDuckNetSuccess : NMDuckNetwork
    {
        public List<Profile> profiles = new List<Profile>();

        public NMJoinDuckNetSuccess()
        {
        }

        public NMJoinDuckNetSuccess(List<Profile> pProfiles) => profiles = pProfiles;

        protected override void OnSerialize()
        {
            _serializedData.Write((byte)profiles.Count);
            for (int index = 0; index < profiles.Count; ++index)
            {
                _serializedData.WriteProfile(profiles[index]);
                _serializedData.Write((ushort)(int)profiles[index].latestGhostIndex);
                _serializedData.WriteTeam(profiles[index].team);
                _serializedData.Write(profiles[index].reservedSpectatorPersona);
                _serializedData.Write((byte)profiles[index].persona.index);
            }
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            profiles = new List<Profile>();
            byte num1 = msg.ReadByte();
            for (int index1 = 0; index1 < num1; ++index1)
            {
                profiles.Add(msg.ReadProfile());
                profiles[index1].latestGhostIndex = (NetIndex16)msg.ReadUShort();
                Team team = msg.ReadTeam();
                if (team != null)
                    profiles[index1].reservedTeam = team;
                sbyte num2 = msg.ReadSByte();
                profiles[index1].reservedSpectatorPersona = num2;
                sbyte index2 = msg.ReadSByte();
                if (index2 >= 0 && index2 < Persona.all.Count<DuckPersona>())
                    profiles[index1].persona = Persona.all.ElementAt<DuckPersona>(index2);
            }
        }
    }
}
