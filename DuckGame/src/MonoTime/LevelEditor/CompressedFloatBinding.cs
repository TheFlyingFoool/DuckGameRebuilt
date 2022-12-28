// Decompiled with JetBrains decompiler
// Type: DuckGame.CompressedFloatBinding
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class CompressedFloatBinding : StateBinding
    {
        private float _range = 1f;

        public override Type type => typeof(int);

        public int GetCompressedFloat(float val)
        {
            int num1 = (int)BitBuffer.GetMaxValue(_bits) / 2;
            if (isRotation)
            {
                if (val < 0f)
                {
                    float num2 = _range / 2f;
                    val = val % -_range + _range;
                }
                val = val % _range / _range;
            }
            else
                val = Maths.Clamp(val, -_range, _range) / _range;
            return (int)Math.Round(val * num1);
        }

        public override object GetNetValue() => GetCompressedFloat(getTyped<float>());

        public override int intValue => GetCompressedFloat(getTyped<float>());

        public override object ReadNetValue(object val)
        {
            float num = (int)val;
            long num2 = BitBuffer.GetMaxValue(_bits) / 2L;
            return num / num2 * _range;
        }

        public override object ReadNetValue(BitBuffer pData)
        {
            float num = (int)pData.ReadBits(type, bits);
            long num2 = BitBuffer.GetMaxValue(_bits) / 2L;
            return num / num2 * _range;
        }

        public CompressedFloatBinding(string field, float range = 1f, int bits = 16, bool isRot = false, bool doLerp = false)
          : base(field, bits, isRot)
        {
            _range = range;
            if (isRot)
                _range = (float)(Math.PI * 2.0f);
            _lerp = doLerp;
        }

        public CompressedFloatBinding(string field, float range, int bits, bool isRot)
          : base(field, bits, isRot)
        {
            _range = range;
            if (!isRot)
                return;
            _range = (float)(Math.PI * 2.0f);
        }

        public CompressedFloatBinding(
          GhostPriority p,
          string field,
          float range = 1f,
          int bits = 16,
          bool isRot = false,
          bool doLerp = false)
          : base(field, bits, isRot)
        {
            _range = range;
            if (isRot)
                _range = (float)(Math.PI * 2.0f);
            _priority = p;
            _lerp = doLerp;
        }
    }
}