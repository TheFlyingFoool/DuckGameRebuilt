namespace DuckGame
{
    public class WireActivatorFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                if ((_thing as WireActivator).action)
                    _value = 1;
                return _value;
            }
            set
            {
                _value = value;
                (_thing as WireActivator).action = (_value & 1U) > 0U;
            }
        }

        public WireActivatorFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 1)
        {
        }
    }
}
