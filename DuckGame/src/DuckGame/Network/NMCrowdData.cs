namespace DuckGame
{
    public class NMCrowdData : NMEvent
    {
        public BitBuffer data;

        public NMCrowdData()
        {
        }

        public NMCrowdData(BitBuffer dat) => data = dat;

        protected override void OnSerialize() => _serializedData.Write(data, true);

        public override void OnDeserialize(BitBuffer d) => data = d.ReadBitBuffer();
    }
}
