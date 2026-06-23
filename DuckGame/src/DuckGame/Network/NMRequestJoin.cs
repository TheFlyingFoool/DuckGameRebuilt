using System.Collections.Generic;

namespace DuckGame
{
    [FixedNetworkID(14)]
    public class NMRequestJoin : NMDuckNetwork
    {
        public string password;
        public Info info;
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
            info = Info.Construct();
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
            _serializedData.Write(true); //this is here to signify this is a rebuilt user -Lucky
        }

        public override void OnDeserialize(BitBuffer d)
        {
            info = Info.Deserialize(d);
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
            try//try catch just in case you never know -Lucky
            {
                isRebuiltUser = d.ReadBool();
            }
            catch
            {
                isRebuiltUser = false;
            }
        }
        public bool isRebuiltUser;

        public struct Info
        {
            public byte roomFlippers;
            public int flagIndex;
            public bool hasCustomHats;
            public bool parentalControlsActive;

            public static Info Construct() => new Info()
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

            public static Info Deserialize(BitBuffer pBuffer) => new Info()
            {
                roomFlippers = pBuffer.ReadByte(),
                flagIndex = pBuffer.ReadInt(),
                hasCustomHats = pBuffer.ReadBool(),
                parentalControlsActive = pBuffer.ReadBool()
            };
        }
    }
}
