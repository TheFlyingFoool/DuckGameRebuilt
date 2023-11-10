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
