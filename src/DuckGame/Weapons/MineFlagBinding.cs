// Decompiled with JetBrains decompiler
// Type: DuckGame.MineFlagBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
                this._value = (ushort)0;
                Mine thing = this._thing as Mine;
                if (thing._pin)
                    this._value |= (ushort)8;
                if (thing._armed)
                    this._value |= (ushort)4;
                if (thing._clicked)
                    this._value |= (ushort)2;
                if (thing._thrown)
                    this._value |= (ushort)1;
                return this._value;
            }
            set
            {
                this._value = value;
                Mine thing = this._thing as Mine;
                thing._pin = ((uint)this._value & 8U) > 0U;
                thing._armed = ((uint)this._value & 4U) > 0U;
                thing._clicked = ((uint)this._value & 2U) > 0U;
                thing._thrown = ((uint)this._value & 1U) > 0U;
            }
        }

        public MineFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 4)
        {
        }
    }
}
