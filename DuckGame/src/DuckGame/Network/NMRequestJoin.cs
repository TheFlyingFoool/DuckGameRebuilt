// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRequestJoin
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            wasInvited = pWasInvited;
            localID = pLocalID;
            names = pNames;
            info = NMRequestJoin.Info.Construct();
            password = pPassword;
            personas = pPersonas;
        }

        protected override void OnSerialize()
        {
            info.Serialize(_serializedData);
            _serializedData.Write(wasInvited);
            _serializedData.Write((byte)0);
            _serializedData.Write(password);
            _serializedData.Write(localID);
            _serializedData.Write((byte)names.Count);
            foreach (string name in names)
                _serializedData.Write(name);
            foreach (byte persona in personas)
                _serializedData.Write(persona);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            info = NMRequestJoin.Info.Deserialize(d);
            wasInvited = d.ReadBool();
            int num = d.ReadByte();
            if (num == 0)
            {
                password = d.ReadString();
                localID = d.ReadULong();
                num = d.ReadByte();
            }
            names = new List<string>();
            personas = new List<byte>();
            for (int index = 0; index < num; ++index)
                names.Add(d.ReadString());
            for (int index = 0; index < num; ++index)
                personas.Add(d.ReadByte());
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
                pBuffer.Write(roomFlippers);
                pBuffer.Write(flagIndex);
                pBuffer.Write(hasCustomHats);
                pBuffer.Write(parentalControlsActive);
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
