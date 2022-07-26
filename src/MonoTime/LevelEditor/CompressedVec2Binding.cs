// Decompiled with JetBrains decompiler
// Type: DuckGame.CompressedVec2Binding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class CompressedVec2Binding : StateBinding
    {
        private int _range;

        public override int bits => 32;

        public override System.Type type => typeof(int);

        public override object GetNetValue() => (object)CompressedVec2Binding.GetCompressedVec2(this.getTyped<Vec2>(), this._range);

        public static int GetCompressedVec2(Vec2 val, int range = 2147483647)
        {
            if ((double)Math.Abs(val.x) < 1.0000000116861E-07)
                val.x = 0.0f;
            if ((double)Math.Abs(val.y) < 1.0000000116861E-07)
                val.y = 0.0f;
            if (range != int.MaxValue)
            {
                float num = (float)((int)short.MaxValue / range);
                val.x = Maths.Clamp(val.x, (float)-range, (float)range) * num;
                val.y = Maths.Clamp(val.y, (float)-range, (float)range) * num;
            }
            return (int)((long)(ushort)Maths.Clamp((int)Math.Round((double)val.x), (int)short.MinValue, (int)short.MaxValue) << 16 | (long)(ushort)Maths.Clamp((int)Math.Round((double)val.y), (int)short.MinValue, (int)short.MaxValue));
        }

        public override int intValue => CompressedVec2Binding.GetCompressedVec2((Vec2)this.classValue, this._range);

        public override object ReadNetValue(object val) => (object)CompressedVec2Binding.GetUncompressedVec2((int)val, this._range);

        public override object ReadNetValue(BitBuffer pData) => (object)CompressedVec2Binding.GetUncompressedVec2((int)pData.ReadBits(this.type, this.bits), this._range);

        public static Vec2 GetUncompressedVec2(int val, int range = 2147483647)
        {
            int num1 = val;
            Vec2 uncompressedVec2 = new Vec2((float)(short)(num1 >> 16 & (int)ushort.MaxValue), (float)(short)(num1 & (int)ushort.MaxValue));
            if (range != int.MaxValue)
            {
                float num2 = (float)((int)short.MaxValue / range);
                uncompressedVec2.x /= num2;
                uncompressedVec2.y /= num2;
            }
            return uncompressedVec2;
        }

        public CompressedVec2Binding(string field, int range = 2147483647, bool isvelocity = false, bool doLerp = false)
          : base(field, vel: isvelocity)
        {
            this._range = range;
            this._lerp = doLerp;
        }

        public CompressedVec2Binding(
          GhostPriority p,
          string field,
          int range = 2147483647,
          bool isvelocity = false,
          bool doLerp = false)
          : base(field, vel: isvelocity)
        {
            this._range = range;
            this._priority = p;
            this._lerp = doLerp;
        }

        public CompressedVec2Binding(string field, int range, bool doLerp)
          : base(field)
        {
            this._range = range;
            this._lerp = doLerp;
        }

        public CompressedVec2Binding(string field, int range)
          : base(field)
        {
            this._range = range;
        }
    }
}
