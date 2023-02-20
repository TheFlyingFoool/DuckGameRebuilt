// Decompiled with JetBrains decompiler
// Type: DuckGame.BananaFlagBinding
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
