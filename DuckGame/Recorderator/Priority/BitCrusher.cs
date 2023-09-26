using System;
using System.Collections;
using System.Linq;

namespace DuckGame
{
    //so basically, less bytes per float but also less precision
    //i think it should be fine (it won't)
    public static class BitCrusher
    {
        /// <summary>
        /// Put in a bitarray, tell it the initial and end index of bits it should use, the current float value and how much this value can go to
        /// </summary>
        /// <param name="bitArray"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="value"></param>
        /// <param name="upperValue"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void CompressFloat(BitArray bitArray, int startIndex, int endIndex, float value, float upperValue)
        {
            if (startIndex < 0 || startIndex >= bitArray.Length || endIndex < 0 || endIndex >= bitArray.Length || startIndex > endIndex)
            {
                throw new ArgumentOutOfRangeException("Invalid startIndex or endIndex");
            }

            value = Maths.Clamp(value, 0, upperValue);

            int numBits = endIndex - startIndex + 1;
            int numSteps = 1 << numBits;
            int compressedValue = (int)Math.Floor((value / upperValue) * numSteps);

            for (int i = 0; i < numBits; i++)
            {
                bitArray[startIndex + i] = (compressedValue & (1 << i)) != 0;
            }
        }

        /// <summary>
        /// Opposite as CompressFloat
        /// </summary>
        /// <param name="bitArray"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="upperValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float DecompressFloat(BitArray bitArray, int startIndex, int endIndex, float upperValue)
        {
            if (startIndex < 0 || startIndex >= bitArray.Length || endIndex < 0 || endIndex >= bitArray.Length || startIndex > endIndex)
            {
                throw new ArgumentOutOfRangeException("Invalid startIndex or endIndex");
            }

            int numBits = endIndex - startIndex + 1;
            int numSteps = 1 << numBits;

            int compressedValue = 0;
            for (int i = 0; i < numBits; i++)
            {
                if (bitArray[startIndex + i])
                {
                    compressedValue |= (1 << i);
                }
            }

            return (float)compressedValue / numSteps * upperValue;
        }
        public static BitArray ByteToBitArray(byte b)
        {
            return new BitArray(new byte[] { b });
        }
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
        public static ushort BitArrayToUShort(BitArray arr)
        {
            ushort divide = 32768;
            ushort xd = 0;

            
            for (int i = 0; i < 16; i++)
            {
                if (arr[i]) xd += divide;
                divide /= 2;
            }
            return xd;
        }
        public static BitArray UShortIntoArray(ushort val, ref BitArray arr)
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
        // Convert an int to a BitArray -ChatGPT
        public static BitArray IntToBitArray(int number)
        {
            byte[] bytes = BitConverter.GetBytes(number);
            BitArray bitArray = new BitArray(bytes);
            return bitArray;
        }

        // Convert a BitArray to an int -ChatGPT
        public static int BitArrayToInt(BitArray bitArray)
        {
            if (bitArray.Length != 32) // Assuming 32 bits for an int -ChatGPT
            {
                throw new ArgumentException("BitArray length must be 32 for converting to int.");
            }

            byte[] bytes = new byte[4];
            bitArray.CopyTo(bytes, 0);
            int number = BitConverter.ToInt32(bytes, 0);
            return number;
        }
        public static ushort FloatToUShort(float v, int range = 1000)
        {
            float f = range / 65535f;
            return (ushort)Math.Round(v / f);
        }

        public static float UShortToFloat(ushort v, int range = 1000, int rounding = 2)
        {
            float f = range / 65535f;
            return (float)Math.Round(v * f, rounding);
        }

        public static byte FloatToByte(float v, float range = 1)
        {
            float f = range / 255f;
            return (byte)Math.Round(v / f);
        }
        public static float ByteToFloat(byte v, float range = 1, int rounding = 2)
        {
            float f = range / 255f;
            return (float)Math.Round(v * f, rounding);
        }
    }
}
