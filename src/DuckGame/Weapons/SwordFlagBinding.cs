// Decompiled with JetBrains decompiler
// Type: DuckGame.SwordFlagBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class SwordFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                this._value = (ushort)0;
                Sword thing = this._thing as Sword;
                if (thing._jabStance)
                    this._value |= (ushort)16;
                if (thing._crouchStance)
                    this._value |= (ushort)8;
                if (thing._slamStance)
                    this._value |= (ushort)4;
                if (thing._swinging)
                    this._value |= (ushort)2;
                if (thing._volatile)
                    this._value |= (ushort)1;
                return this._value;
            }
            set
            {
                this._value = value;
                Sword thing = this._thing as Sword;
                thing._jabStance = ((uint)this._value & 16U) > 0U;
                thing._crouchStance = ((uint)this._value & 8U) > 0U;
                thing._slamStance = ((uint)this._value & 4U) > 0U;
                thing._swinging = ((uint)this._value & 2U) > 0U;
                thing._volatile = ((uint)this._value & 1U) > 0U;
            }
        }

        public SwordFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 5)
        {
        }
    }
}
