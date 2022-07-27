// Decompiled with JetBrains decompiler
// Type: DuckGame.BananaFlagBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
                this._value = 0;
                Banana thing = this._thing as Banana;
                if (thing._pin)
                    this._value |= 2;
                if (thing._thrown)
                    this._value |= 1;
                return this._value;
            }
            set
            {
                this._value = value;
                Banana thing = this._thing as Banana;
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
