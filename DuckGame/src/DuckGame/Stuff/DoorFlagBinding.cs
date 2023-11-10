namespace DuckGame
{
    public class DoorFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                Door thing = _thing as Door;
                if (thing._didJiggle)
                    _value |= 8;
                if (thing._jammed)
                    _value |= 4;
                if (thing._destroyed)
                    _value |= 2;
                if (thing.locked)
                    _value |= 1;
                return _value;
            }
            set
            {
                _value = value;
                Door thing = _thing as Door;
                thing._didJiggle = (_value & 8U) > 0U;
                thing._jammed = (_value & 4U) > 0U;
                thing._destroyed = (_value & 2U) > 0U;
                thing.locked = (_value & 1U) > 0U;
            }
        }

        public DoorFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 4)
        {
        }
    }
}
