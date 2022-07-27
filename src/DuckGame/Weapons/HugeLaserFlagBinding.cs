// Decompiled with JetBrains decompiler
// Type: DuckGame.HugeLaserFlagBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class HugeLaserFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                this._value = 0;
                HugeLaser thing = this._thing as HugeLaser;
                if (thing._charging)
                    this._value |= 4;
                if (thing._fired)
                    this._value |= 2;
                if (thing.doBlast)
                    this._value |= 1;
                return this._value;
            }
            set
            {
                this._value = value;
                HugeLaser thing = this._thing as HugeLaser;
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
