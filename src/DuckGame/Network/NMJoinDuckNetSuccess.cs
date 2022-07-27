// Decompiled with JetBrains decompiler
// Type: DuckGame.NMJoinDuckNetSuccess
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public NMJoinDuckNetSuccess(List<Profile> pProfiles) => this.profiles = pProfiles;

        protected override void OnSerialize()
        {
            this._serializedData.Write((byte)this.profiles.Count);
            for (int index = 0; index < this.profiles.Count; ++index)
            {
                this._serializedData.WriteProfile(this.profiles[index]);
                this._serializedData.Write((ushort)(int)this.profiles[index].latestGhostIndex);
                this._serializedData.WriteTeam(this.profiles[index].team);
                this._serializedData.Write(this.profiles[index].reservedSpectatorPersona);
                this._serializedData.Write((byte)this.profiles[index].persona.index);
            }
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            this.profiles = new List<Profile>();
            byte num1 = msg.ReadByte();
            for (int index1 = 0; index1 < num1; ++index1)
            {
                this.profiles.Add(msg.ReadProfile());
                this.profiles[index1].latestGhostIndex = (NetIndex16)msg.ReadUShort();
                Team team = msg.ReadTeam();
                if (team != null)
                    this.profiles[index1].reservedTeam = team;
                sbyte num2 = msg.ReadSByte();
                this.profiles[index1].reservedSpectatorPersona = num2;
                sbyte index2 = msg.ReadSByte();
                if (index2 >= 0 && index2 < Persona.all.Count<DuckPersona>())
                    this.profiles[index1].persona = Persona.all.ElementAt<DuckPersona>(index2);
            }
        }
    }
}
