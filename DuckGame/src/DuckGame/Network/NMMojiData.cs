namespace DuckGame
{
    public class NMMojiData : NMDuckNetwork
    {
        public string data;
        public string name;

        public NMMojiData()
        {
        }

        public NMMojiData(string dat, string nam)
        {
            data = dat;
            name = nam;
        }

        protected override void OnSerialize()
        {
            _serializedData.Write(name);
            _serializedData.Write(data);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            name = d.ReadString();
            data = d.ReadString();
        }
    }
}
