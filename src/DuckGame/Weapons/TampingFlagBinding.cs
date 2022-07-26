// Decompiled with JetBrains decompiler
// Type: DuckGame.TampingFlagBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class TampingFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                this._value = (ushort)0;
                TampingWeapon thing = this._thing as TampingWeapon;
                if (thing._tamped)
                    this._value |= (ushort)4;
                if (thing.tamping)
                    this._value |= (ushort)2;
                if (thing._rotating)
                    this._value |= (ushort)1;
                return this._value;
            }
            set
            {
                this._value = value;
                TampingWeapon thing = this._thing as TampingWeapon;
                thing._tamped = ((uint)this._value & 4U) > 0U;
                thing.tamping = ((uint)this._value & 2U) > 0U;
                thing._rotating = ((uint)this._value & 1U) > 0U;
            }
        }

        public TampingFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 3)
        {
        }
    }
}
