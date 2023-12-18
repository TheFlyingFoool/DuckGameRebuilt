namespace DuckGame
{
    public class WireMountFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                if ((_thing as WireMount).action)
                    _value = 1;
                return _value;
            }
            set
            {
                _value = value;
                (_thing as WireMount).action = (_value & 1U) > 0U;
            }
        }

        public WireMountFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 1)
        {
        }
    }
}
