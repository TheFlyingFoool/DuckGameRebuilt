// Decompiled with JetBrains decompiler
// Type: DuckGame.DoorFlagBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
                this._value = (ushort)0;
                Door thing = this._thing as Door;
                if (thing._didJiggle)
                    this._value |= (ushort)8;
                if (thing._jammed)
                    this._value |= (ushort)4;
                if (thing._destroyed)
                    this._value |= (ushort)2;
                if (thing.locked)
                    this._value |= (ushort)1;
                return this._value;
            }
            set
            {
                this._value = value;
                Door thing = this._thing as Door;
                thing._didJiggle = ((uint)this._value & 8U) > 0U;
                thing._jammed = ((uint)this._value & 4U) > 0U;
                thing._destroyed = ((uint)this._value & 2U) > 0U;
                thing.locked = ((uint)this._value & 1U) > 0U;
            }
        }

        public DoorFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 4)
        {
        }
    }
}
