// Decompiled with JetBrains decompiler
// Type: DuckGame.CompressedFloatBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class CompressedFloatBinding : StateBinding
    {
        private float _range = 1f;

        public override System.Type type => typeof(int);

        public int GetCompressedFloat(float val)
        {
            int num1 = (int)BitBuffer.GetMaxValue(this._bits) / 2;
            if (this.isRotation)
            {
                if ((double)val < 0.0)
                {
                    double num2 = _range / 2.0;
                    val = val % -this._range + this._range;
                }
                val = val % this._range / this._range;
            }
            else
                val = Maths.Clamp(val, -this._range, this._range) / this._range;
            return (int)Math.Round((double)val * num1);
        }

        public override object GetNetValue() => this.GetCompressedFloat(this.getTyped<float>());

        public override int intValue => this.GetCompressedFloat(this.getTyped<float>());

        public override object ReadNetValue(object val) => (float)((int)val / (double)(BitBuffer.GetMaxValue(this._bits) / 2L) * _range);

        public override object ReadNetValue(BitBuffer pData) => (float)((int)pData.ReadBits(this.type, this.bits) / (double)(BitBuffer.GetMaxValue(this._bits) / 2L) * _range);

        public CompressedFloatBinding(string field, float range = 1f, int bits = 16, bool isRot = false, bool doLerp = false)
          : base(field, bits, isRot)
        {
            this._range = range;
            if (isRot)
                this._range = 6.283185f;
            this._lerp = doLerp;
        }

        public CompressedFloatBinding(string field, float range, int bits, bool isRot)
          : base(field, bits, isRot)
        {
            this._range = range;
            if (!isRot)
                return;
            this._range = 6.283185f;
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
            this._range = range;
            if (isRot)
                this._range = 6.283185f;
            this._priority = p;
            this._lerp = doLerp;
        }
    }
}
