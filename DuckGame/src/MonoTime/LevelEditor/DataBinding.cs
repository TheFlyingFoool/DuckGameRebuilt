namespace DuckGame
{
    public class DataBinding : StateBinding
    {
        public DataBinding(string field)
          : base(field)
        {
        }

        public override object ReadNetValue(object val) => val;

        public override object ReadNetValue(BitBuffer pData) => pData.ReadBitBuffer();
    }
}
