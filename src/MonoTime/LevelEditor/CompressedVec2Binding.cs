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

        public override object GetNetValue() => CompressedVec2Binding.GetCompressedVec2(getTyped<Vec2>(), _range);

        public static int GetCompressedVec2(Vec2 val, int range = 2147483647)
        {
            if (Math.Abs(val.x) < 1E-07f)
                val.x = 0f;
            if (Math.Abs(val.y) < 1E-07f)
                val.y = 0f;
            if (range != int.MaxValue)
            {
                float num = short.MaxValue / range;
                val.x = Maths.Clamp(val.x, -range, range) * num;
                val.y = Maths.Clamp(val.y, -range, range) * num;
            }
            return (int)((long)(ushort)Maths.Clamp((int)Math.Round(val.x), short.MinValue, short.MaxValue) << 16 | (ushort)Maths.Clamp((int)Math.Round(val.y), short.MinValue, short.MaxValue));
        }

        public override int intValue => CompressedVec2Binding.GetCompressedVec2((Vec2)classValue, _range);

        public override object ReadNetValue(object val) => CompressedVec2Binding.GetUncompressedVec2((int)val, _range);

        public override object ReadNetValue(BitBuffer pData) => CompressedVec2Binding.GetUncompressedVec2((int)pData.ReadBits(type, bits), _range);

        public static Vec2 GetUncompressedVec2(int val, int range = 2147483647)
        {
            int num1 = val;
            Vec2 uncompressedVec2 = new Vec2((short)(num1 >> 16 & ushort.MaxValue), (short)(num1 & ushort.MaxValue));
            if (range != int.MaxValue)
            {
                float num2 = short.MaxValue / range;
                uncompressedVec2.x /= num2;
                uncompressedVec2.y /= num2;
            }
            return uncompressedVec2;
        }

        public CompressedVec2Binding(string field, int range = 2147483647, bool isvelocity = false, bool doLerp = false)
          : base(field, vel: isvelocity)
        {
            _range = range;
            _lerp = doLerp;
        }

        public CompressedVec2Binding(
          GhostPriority p,
          string field,
          int range = 2147483647,
          bool isvelocity = false,
          bool doLerp = false)
          : base(field, vel: isvelocity)
        {
            _range = range;
            _priority = p;
            _lerp = doLerp;
        }

        public CompressedVec2Binding(string field, int range, bool doLerp)
          : base(field)
        {
            _range = range;
            _lerp = doLerp;
        }

        public CompressedVec2Binding(string field, int range)
          : base(field)
        {
            _range = range;
        }
    }
}
