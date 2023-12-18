namespace DuckGame
{
    public class RagdollFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                Ragdoll thing = _thing as Ragdoll;
                if (thing.inSleepingBag)
                    _value |= 16;
                if (thing.solid)
                    _value |= 8;
                if (thing.enablePhysics)
                    _value |= 4;
                if (thing.active)
                    _value |= 2;
                if (thing.visible)
                    _value |= 1;
                return _value;
            }
            set
            {
                _value = value;
                Ragdoll thing = _thing as Ragdoll;
                thing.inSleepingBag = (_value & 16U) > 0U;
                thing.solid = (_value & 8U) > 0U;
                thing.enablePhysics = (_value & 4U) > 0U;
                thing.active = (_value & 2U) > 0U;
                thing.visible = (_value & 1U) > 0U;
            }
        }

        public RagdollFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 5)
        {
        }
    }
}
