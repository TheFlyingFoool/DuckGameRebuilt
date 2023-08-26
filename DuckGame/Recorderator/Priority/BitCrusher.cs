using System;
using System.Collections;

namespace DuckGame
{
    //so basically, less bytes per float but also less precision
    //i think it should be fine (it won't)
    public static class BitCrusher
    {
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
            float f = range / (negative ? 32767.5f : 65535);
            return (ushort)Math.Round(v / f);
        }
        public static float UShortToFloat(ushort v, int range = 1000, bool negative = true, int rounding = 2)
        {
            float f = range / (negative ? 32767.5f : 65535);
            return (float)Math.Round(v * f, rounding);
        }
    }
}
