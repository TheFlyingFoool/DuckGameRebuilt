namespace DuckGame
{
    public class BananaFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                Banana thing = _thing as Banana;
                if (thing._pin)
                    _value |= 2;
                if (thing._thrown)
                    _value |= 1;
                return _value;
            }
            set
            {
                _value = value;
                Banana thing = _thing as Banana;
                thing._pin = (_value & 2U) > 0U;
                thing._thrown = (_value & 1U) > 0U;
            }
        }

        public BananaFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 2)
        {
        }
    }
}
