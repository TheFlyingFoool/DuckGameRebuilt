namespace DuckGame
{
    public class NetSoundBinding : StateBinding
    {
        public override System.Type type => typeof(byte);

        public override object classValue
        {
            get => byteValue;
            set => byteValue = (byte)value;
        }

        public override byte byteValue
        {
            get => (byte)(_accessor.getAccessor(_thing) as NetSoundEffect).index;
            set => (_accessor.getAccessor(_thing) as NetSoundEffect).index = value;
        }

        public NetSoundBinding(string field)
          : base(field, 2)
        {
        }

        public NetSoundBinding(GhostPriority p, string field)
          : base(field, 2)
        {
            _priority = p;
        }
    }
}
