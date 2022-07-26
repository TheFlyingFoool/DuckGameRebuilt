// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckFlagBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DuckFlagBinding : StateFlagBase
    {
        public override ushort ushortValue
        {
            get
            {
                this._value = (ushort)0;
                Duck thing = this._thing as Duck;
                if (thing.invincible)
                    this._value |= (ushort)1024;
                if (thing.crouch)
                    this._value |= (ushort)512;
                if (thing.sliding)
                    this._value |= (ushort)256;
                if (thing.jumping)
                    this._value |= (ushort)128;
                if (thing._hovering)
                    this._value |= (ushort)64;
                if (thing.immobilized)
                    this._value |= (ushort)32;
                if (thing._canFire)
                    this._value |= (ushort)16;
                if (thing.afk)
                    this._value |= (ushort)8;
                if (thing.listening)
                    this._value |= (ushort)4;
                if (thing.beammode)
                    this._value |= (ushort)2;
                if (thing.eyesClosed)
                    this._value |= (ushort)1;
                return this._value;
            }
            set
            {
                this._value = value;
                Duck thing = this._thing as Duck;
                thing.invincible = ((uint)this._value & 1024U) > 0U;
                thing.crouch = ((uint)this._value & 512U) > 0U;
                thing.sliding = ((uint)this._value & 256U) > 0U;
                thing.jumping = ((uint)this._value & 128U) > 0U;
                thing._hovering = ((uint)this._value & 64U) > 0U;
                thing.immobilized = ((uint)this._value & 32U) > 0U;
                thing._canFire = ((uint)this._value & 16U) > 0U;
                thing.afk = ((uint)this._value & 8U) > 0U;
                thing.listening = ((uint)this._value & 4U) > 0U;
                thing.beammode = ((uint)this._value & 2U) > 0U;
                thing.eyesClosed = ((uint)this._value & 1U) > 0U;
            }
        }

        public DuckFlagBinding(GhostPriority p)
          : base(p, 11)
        {
        }
    }
}
