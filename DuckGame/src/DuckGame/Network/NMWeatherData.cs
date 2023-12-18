namespace DuckGame
{
    public class NMWeatherData : NMEvent
    {
        public BitBuffer data;

        public NMWeatherData()
        {
        }

        public NMWeatherData(BitBuffer dat) => data = dat;

        protected override void OnSerialize() => _serializedData.Write(data, true);

        public override void OnDeserialize(BitBuffer d) => data = d.ReadBitBuffer();
    }
}
