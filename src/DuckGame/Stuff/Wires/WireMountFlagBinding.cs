// Decompiled with JetBrains decompiler
// Type: DuckGame.WireMountFlagBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WireMountFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                this._value = 0;
                if ((this._thing as WireMount).action)
                    this._value = 1;
                return this._value;
            }
            set
            {
                this._value = value;
                (this._thing as WireMount).action = (_value & 1U) > 0U;
            }
        }

        public WireMountFlagBinding(GhostPriority p = GhostPriority.Normal)
          : base(p, 1)
        {
        }
    }
}
