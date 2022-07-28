// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRequestJoin
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [FixedNetworkID(14)]
    public class NMRequestJoin : NMDuckNetwork
    {
        public string password;
        public NMRequestJoin.Info info;
        public bool wasInvited;
        public ulong localID;
        public List<string> names = new List<string>();
        public List<byte> personas = new List<byte>();

        public NMRequestJoin()
        {
        }

        public NMRequestJoin(
          List<string> pNames,
          List<byte> pPersonas,
          bool pWasInvited = false,
          string pPassword = "",
          ulong pLocalID = 0)
        {
            this.wasInvited = pWasInvited;
            this.localID = pLocalID;
            this.names = pNames;
            this.info = NMRequestJoin.Info.Construct();
            this.password = pPassword;
            this.personas = pPersonas;
        }

        protected override void OnSerialize()
        {
            this.info.Serialize(this._serializedData);
            this._serializedData.Write(this.wasInvited);
            this._serializedData.Write((byte)0);
            this._serializedData.Write(this.password);
            this._serializedData.Write(this.localID);
            this._serializedData.Write((byte)this.names.Count);
            foreach (string name in this.names)
                this._serializedData.Write(name);
            foreach (byte persona in this.personas)
                this._serializedData.Write(persona);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            this.info = NMRequestJoin.Info.Deserialize(d);
            this.wasInvited = d.ReadBool();
            int num = d.ReadByte();
            if (num == 0)
            {
                this.password = d.ReadString();
                this.localID = d.ReadULong();
                num = d.ReadByte();
            }
            this.names = new List<string>();
            this.personas = new List<byte>();
            for (int index = 0; index < num; ++index)
                this.names.Add(d.ReadString());
            for (int index = 0; index < num; ++index)
                this.personas.Add(d.ReadByte());
        }

        public struct Info
        {
            public byte roomFlippers;
            public int flagIndex;
            public bool hasCustomHats;
            public bool parentalControlsActive;

            public static NMRequestJoin.Info Construct() => new NMRequestJoin.Info()
            {
                roomFlippers = Profile.CalculateLocalFlippers(),
                flagIndex = Global.data.flag,
                hasCustomHats = Teams.core.extraTeams.Count > 0,
                parentalControlsActive = false   //ParentalControls.AreParentalControlsActive()
            };

            public void Serialize(BitBuffer pBuffer)
            {
                pBuffer.Write(this.roomFlippers);
                pBuffer.Write(this.flagIndex);
                pBuffer.Write(this.hasCustomHats);
                pBuffer.Write(this.parentalControlsActive);
            }

            public static NMRequestJoin.Info Deserialize(BitBuffer pBuffer) => new NMRequestJoin.Info()
            {
                roomFlippers = pBuffer.ReadByte(),
                flagIndex = pBuffer.ReadInt(),
                hasCustomHats = pBuffer.ReadBool(),
                parentalControlsActive = pBuffer.ReadBool()
            };
        }
    }
}
