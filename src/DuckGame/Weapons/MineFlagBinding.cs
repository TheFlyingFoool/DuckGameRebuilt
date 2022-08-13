// Decompiled with JetBrains decompiler
// Type: DuckGame.MineFlagBinding
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class MineFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                Mine thing = _thing as Mine;
                if (thing._pin)
                    _value |= 8;
                if (thing._armed)
                    _value |= 4;
                if (thing._clicked)
                    _value |= 2;
                if (thing._thrown)
                    _value |= 1;
                return _value;
            }
            set
            {
                _value = value;
                Mine thing = _thing as Mine;
                thing._pin = (_value & 8U) > 0U;
                thing._armed = (_value & 4U) > 0U;
                thing._clicked = (_value & 2U) > 0U;
                thing._thrown = (_value & 1U) > 0U;
            }
        }

        public MineFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 4)
        {
        }
    }
}
