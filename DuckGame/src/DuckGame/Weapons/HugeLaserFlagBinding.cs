namespace DuckGame
{
    public class HugeLaserFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                HugeLaser thing = _thing as HugeLaser;
                if (thing._charging)
                    _value |= 4;
                if (thing._fired)
                    _value |= 2;
                if (thing.doBlast)
                    _value |= 1;
                return _value;
            }
            set
            {
                _value = value;
                HugeLaser thing = _thing as HugeLaser;
                thing._charging = (_value & 4U) > 0U;
                thing._fired = (_value & 2U) > 0U;
                thing.doBlast = (_value & 1U) > 0U;
            }
        }

        public HugeLaserFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 3)
        {
        }
    }
}
