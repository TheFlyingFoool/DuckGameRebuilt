namespace DuckGame
{
    public class DuckFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                Duck thing = _thing as Duck;
                if (thing.invincible)
                    _value |= 1024;
                if (thing.crouch)
                    _value |= 512;
                if (thing.sliding)
                    _value |= 256;
                if (thing.jumping)
                    _value |= 128;
                if (thing._hovering)
                    _value |= 64;
                if (thing.immobilized)
                    _value |= 32;
                if (thing._canFire)
                    _value |= 16;
                if (thing.afk)
                    _value |= 8;
                if (thing.listening)
                    _value |= 4;
                if (thing.beammode)
                    _value |= 2;
                if (thing.eyesClosed)
                    _value |= 1;
                return _value;
            }
            set
            {
                _value = value;
                Duck thing = _thing as Duck;
                thing.invincible = (_value & 1024U) > 0U;
                thing.crouch = (_value & 512U) > 0U;
                thing.sliding = (_value & 256U) > 0U;
                thing.jumping = (_value & 128U) > 0U;
                thing._hovering = (_value & 64U) > 0U;
                thing.immobilized = (_value & 32U) > 0U;
                thing._canFire = (_value & 16U) > 0U;
                thing.afk = (_value & 8U) > 0U;
                thing.listening = (_value & 4U) > 0U;
                thing.beammode = (_value & 2U) > 0U;
                thing.eyesClosed = (_value & 1U) > 0U;
            }
        }

        public DuckFlagBinding(GhostPriority p)
          : base(p, 11)
        {
        }
    }
}
