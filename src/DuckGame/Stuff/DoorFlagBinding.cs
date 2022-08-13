// Decompiled with JetBrains decompiler
// Type: DuckGame.DoorFlagBinding
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
