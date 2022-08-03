// Decompiled with JetBrains decompiler
// Type: DuckGame.CoilGunFlagBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CoilGunFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                _value = 0;
                CoilGun thing = _thing as CoilGun;
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
                CoilGun thing = _thing as CoilGun;
                thing._charging = (_value & 4U) > 0U;
                thing._fired = (_value & 2U) > 0U;
                thing.doBlast = (_value & 1U) > 0U;
            }
        }

        public CoilGunFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 3)
        {
        }
    }
}
