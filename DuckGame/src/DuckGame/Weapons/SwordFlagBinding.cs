namespace DuckGame
{
    public class SwordFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                Sword thing = _thing as Sword;
                if (thing._jabStance)
                    _value |= 16;
                if (thing._crouchStance)
                    _value |= 8;
                if (thing._slamStance)
                    _value |= 4;
                if (thing._swinging)
                    _value |= 2;
                if (thing._volatile)
                    _value |= 1;
                return _value;
            }
            set
            {
                _value = value;
                Sword thing = _thing as Sword;
                thing._jabStance = (_value & 16U) > 0U;
                thing._crouchStance = (_value & 8U) > 0U;
                thing._slamStance = (_value & 4U) > 0U;
                thing._swinging = (_value & 2U) > 0U;
                thing._volatile = (_value & 1U) > 0U;
            }
        }

        public SwordFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 5)
        {
        }
    }
}
