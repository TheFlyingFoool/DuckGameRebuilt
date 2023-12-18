namespace DuckGame
{
    public class TampingFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                TampingWeapon thing = _thing as TampingWeapon;
                if (thing._tamped)
                    _value |= 4;
                if (thing.tamping)
                    _value |= 2;
                if (thing._rotating)
                    _value |= 1;
                return _value;
            }
            set
            {
                _value = value;
                TampingWeapon thing = _thing as TampingWeapon;
                thing._tamped = (_value & 4U) > 0U;
                thing.tamping = (_value & 2U) > 0U;
                thing._rotating = (_value & 1U) > 0U;
            }
        }

        public TampingFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 3)
        {
        }
    }
}
