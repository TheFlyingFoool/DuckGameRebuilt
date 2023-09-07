using System;
using System.Collections;
using System.Linq;

namespace DuckGame
{
    //so basically, less bytes per float but also less precision
    //i think it should be fine (it won't)
    public static class BitCrusher
    {
        public static byte BitArrayToByte(BitArray array)
        {
            byte[] b = new byte[1];
            array.CopyTo(b, 0);
            return b.First();
        }
        public static byte[] BitArrayToBytes(BitArray array)
        {
            byte[] b = new byte[Maths.Clamp(array.Length / 8, 1, 100)];
            array.CopyTo(b, 0);
            return b;
        }
        public static ushort BitArrayToUShort(BitArray arr, int idx)
        {
            ushort divide = 32768;
            ushort xd = 0;

            
            for (int i = idx; i < idx + 16; i++)
            {
                if (arr[i]) xd += divide;
                divide /= 2;
            }
            return xd;
        }
        public static BitArray UShortIntoArray(ushort val, ref BitArray arr, int idx = 0)
        {
            if ((val & 32768) > 0) arr[0] = true;
            if ((val & 16384) > 0) arr[1] = true;
            if ((val & 8192) > 0) arr[2] = true;
            if ((val & 4096) > 0) arr[3] = true;
            if ((val & 2048) > 0) arr[4] = true;
            if ((val & 1024) > 0) arr[5] = true;
            if ((val & 512) > 0) arr[6] = true;
            if ((val & 256) > 0) arr[7] = true;
            if ((val & 128) > 0) arr[8] = true;
            if ((val & 64) > 0) arr[9] = true;
            if ((val & 32) > 0) arr[10] = true;
            if ((val & 16) > 0) arr[11] = true;
            if ((val & 8) > 0) arr[12] = true;
            if ((val & 4) > 0) arr[13] = true;
            if ((val & 2) > 0) arr[14] = true;
            if ((val & 1) > 0) arr[15] = true;
            return arr;
        }
        public static ushort FloatToUShort(float v, int range = 1000, bool negative = true)
        {
            if (negative)
            {
                float halfRange = range / 2f;
                if (v < -halfRange)
                    v = -halfRange;
                else if (v > halfRange)
                    v = halfRange;

                float f = halfRange / 32767f;
                return (ushort)Math.Round((v + halfRange) / f);
            }
            else
            {
                float f = range / 65535f;
                return (ushort)Math.Round(v / f);
            }
        }

        public static float UShortToFloat(ushort v, int range = 1000, bool negative = true, int rounding = 2)
        {
            if (negative)
            {
                float halfRange = range / 2f;
                float f = halfRange / 32767f;
                float result = (float)Math.Round(v * f, rounding) - halfRange;

                if (result < -halfRange)
                    result = -halfRange;
                else if (result > halfRange)
                    result = halfRange;

                return result;
            }
            else
            {
                float f = range / 65535f;
                return (float)Math.Round(v * f, rounding);
            }
        }

        public static byte FloatToByte(float v, int range = 1, bool negative = true)
        {
            float f = range / (negative ? 128f : 256f);
            return (byte)Math.Round(v / f);
        }
        public static float ByteToFloat(ushort v, int range = 1, bool negative = true, int rounding = 2)
        {
            float f = range / (negative ? 128f : 256f);
            return (float)Math.Round(v * f, rounding);
        }
    }
}
