// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRoomData
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMRoomData : NMDuckNetwork
    {
        public BitBuffer data;
        public Profile profile;

        public NMRoomData()
        {
        }

        public NMRoomData(Profile pProfile, BitBuffer pData)
        {
            data = pData;
            profile = pProfile;
        }

        protected override void OnSerialize()
        {
            _serializedData.WriteProfile(profile);
            _serializedData.Write(data, true);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            profile = d.ReadProfile();
            data = d.ReadBitBuffer();
        }
    }
}
