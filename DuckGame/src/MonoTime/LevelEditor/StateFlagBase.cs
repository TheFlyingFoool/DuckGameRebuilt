namespace DuckGame
{
    public abstract class StateFlagBase : StateBinding
    {
        public ushort _value;

        public override System.Type type => typeof(ushort);

        public override object classValue
        {
            get => ushortValue;
            set => ushortValue = (ushort)value;
        }

        public abstract override ushort ushortValue { get; set; }

        public StateFlagBase(GhostPriority p, int bits)
          : base("multiple", bits)
        {
            _priority = p;
        }

        public override void Connect(Thing t) => _thing = t;
    }
}
