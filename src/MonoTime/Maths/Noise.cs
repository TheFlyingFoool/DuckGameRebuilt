// Decompiled with JetBrains decompiler
// Type: DuckGame.Noise
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    /// <summary>
    /// Implementation of the Perlin simplex noise, an improved Perlin noise algorithm.
    /// Based loosely on SimplexNoise1234 by Stefan Gustavson http://staffwww.itn.liu.se/~stegu/aqsis/aqsis-newnoise/
    /// 
    /// </summary>
    public class Noise
    {
        public static byte[] perm = new byte[512]
        {
       151,
       160,
       137,
       91,
       90,
       15,
       131,
       13,
       201,
       95,
       96,
       53,
       194,
       233,
       7,
       225,
       140,
       36,
       103,
       30,
       69,
       142,
       8,
       99,
       37,
       240,
       21,
       10,
       23,
       190,
       6,
       148,
       247,
       120,
       234,
       75,
       0,
       26,
       197,
       62,
       94,
       252,
       219,
       203,
       117,
       35,
       11,
       32,
       57,
       177,
       33,
       88,
       237,
       149,
       56,
       87,
       174,
       20,
       125,
       136,
       171,
       168,
       68,
       175,
       74,
       165,
       71,
       134,
       139,
       48,
       27,
       166,
       77,
       146,
       158,
       231,
       83,
       111,
       229,
       122,
       60,
       211,
       133,
       230,
       220,
       105,
       92,
       41,
       55,
       46,
       245,
       40,
       244,
       102,
       143,
       54,
       65,
       25,
       63,
       161,
       1,
       216,
       80,
       73,
       209,
       76,
       132,
       187,
       208,
       89,
       18,
       169,
       200,
       196,
       135,
       130,
       116,
       188,
       159,
       86,
       164,
       100,
       109,
       198,
       173,
       186,
       3,
       64,
       52,
       217,
       226,
       250,
       124,
       123,
       5,
       202,
       38,
       147,
       118,
       126,
      byte.MaxValue,
       82,
       85,
       212,
       207,
       206,
       59,
       227,
       47,
       16,
       58,
       17,
       182,
       189,
       28,
       42,
       223,
       183,
       170,
       213,
       119,
       248,
       152,
       2,
       44,
       154,
       163,
       70,
       221,
       153,
       101,
       155,
       167,
       43,
       172,
       9,
       129,
       22,
       39,
       253,
       19,
       98,
       108,
       110,
       79,
       113,
       224,
       232,
       178,
       185,
       112,
       104,
       218,
       246,
       97,
       228,
       251,
       34,
       242,
       193,
       238,
       210,
       144,
       12,
       191,
       179,
       162,
       241,
       81,
       51,
       145,
       235,
       249,
       14,
       239,
       107,
       49,
       192,
       214,
       31,
       181,
       199,
       106,
       157,
       184,
       84,
       204,
       176,
       115,
       121,
       50,
       45,
       127,
       4,
       150,
       254,
       138,
       236,
       205,
       93,
       222,
       114,
       67,
       29,
       24,
       72,
       243,
       141,
       128,
       195,
       78,
       66,
       215,
       61,
       156,
       180,
       151,
       160,
       137,
       91,
       90,
       15,
       131,
       13,
       201,
       95,
       96,
       53,
       194,
       233,
       7,
       225,
       140,
       36,
       103,
       30,
       69,
       142,
       8,
       99,
       37,
       240,
       21,
       10,
       23,
       190,
       6,
       148,
       247,
       120,
       234,
       75,
       0,
       26,
       197,
       62,
       94,
       252,
       219,
       203,
       117,
       35,
       11,
       32,
       57,
       177,
       33,
       88,
       237,
       149,
       56,
       87,
       174,
       20,
       125,
       136,
       171,
       168,
       68,
       175,
       74,
       165,
       71,
       134,
       139,
       48,
       27,
       166,
       77,
       146,
       158,
       231,
       83,
       111,
       229,
       122,
       60,
       211,
       133,
       230,
       220,
       105,
       92,
       41,
       55,
       46,
       245,
       40,
       244,
       102,
       143,
       54,
       65,
       25,
       63,
       161,
       1,
       216,
       80,
       73,
       209,
       76,
       132,
       187,
       208,
       89,
       18,
       169,
       200,
       196,
       135,
       130,
       116,
       188,
       159,
       86,
       164,
       100,
       109,
       198,
       173,
       186,
       3,
       64,
       52,
       217,
       226,
       250,
       124,
       123,
       5,
       202,
       38,
       147,
       118,
       126,
      byte.MaxValue,
       82,
       85,
       212,
       207,
       206,
       59,
       227,
       47,
       16,
       58,
       17,
       182,
       189,
       28,
       42,
       223,
       183,
       170,
       213,
       119,
       248,
       152,
       2,
       44,
       154,
       163,
       70,
       221,
       153,
       101,
       155,
       167,
       43,
       172,
       9,
       129,
       22,
       39,
       253,
       19,
       98,
       108,
       110,
       79,
       113,
       224,
       232,
       178,
       185,
       112,
       104,
       218,
       246,
       97,
       228,
       251,
       34,
       242,
       193,
       238,
       210,
       144,
       12,
       191,
       179,
       162,
       241,
       81,
       51,
       145,
       235,
       249,
       14,
       239,
       107,
       49,
       192,
       214,
       31,
       181,
       199,
       106,
       157,
       184,
       84,
       204,
       176,
       115,
       121,
       50,
       45,
       127,
       4,
       150,
       254,
       138,
       236,
       205,
       93,
       222,
       114,
       67,
       29,
       24,
       72,
       243,
       141,
       128,
       195,
       78,
       66,
       215,
       61,
       156,
       180
        };

        /// <summary>1D simplex noise</summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Generate(float x)
        {
            int num1 = Noise.FastFloor(x);
            int num2 = num1 + 1;
            float x1 = x - num1;
            float x2 = x1 - 1f;
            double num3 = 1.0 - (double)x1 * (double)x1;
            double num4 = num3 * num3;
            float num5 = (float)(num4 * num4) * Noise.grad(Noise.perm[num1 & byte.MaxValue], x1);
            double num6 = 1.0 - (double)x2 * (double)x2;
            double num7 = num6 * num6;
            float num8 = (float)(num7 * num7) * Noise.grad(Noise.perm[num2 & byte.MaxValue], x2);
            return (float)(0.395000010728836 * ((double)num5 + (double)num8));
        }

        /// <summary>2D simplex noise</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float Generate(float x, float y)
        {
            float num1 = (float)(((double)x + (double)y) * 0.366025388240814);
            double x1 = (double)x + (double)num1;
            float x2 = y + num1;
            int num2 = Noise.FastFloor((float)x1);
            int num3 = Noise.FastFloor(x2);
            float num4 = (num2 + num3) * 0.2113249f;
            float num5 = num2 - num4;
            float num6 = num3 - num4;
            float x3 = x - num5;
            float y1 = y - num6;
            int num7;
            int num8;
            if ((double)x3 > (double)y1)
            {
                num7 = 1;
                num8 = 0;
            }
            else
            {
                num7 = 0;
                num8 = 1;
            }
            float x4 = (float)((double)x3 - num7 + 0.211324870586395);
            float y2 = (float)((double)y1 - num8 + 0.211324870586395);
            float x5 = (float)((double)x3 - 1.0 + 0.422649741172791);
            float y3 = (float)((double)y1 - 1.0 + 0.422649741172791);
            int num9 = num2 % 256;
            int index = num3 % 256;
            float num10 = (float)(0.5 - (double)x3 * (double)x3 - (double)y1 * (double)y1);
            float num11;
            if ((double)num10 < 0.0)
            {
                num11 = 0f;
            }
            else
            {
                float num12 = num10 * num10;
                num11 = num12 * num12 * Noise.grad(Noise.perm[num9 + Noise.perm[index]], x3, y1);
            }
            float num13 = (float)(0.5 - (double)x4 * (double)x4 - (double)y2 * (double)y2);
            float num14;
            if ((double)num13 < 0.0)
            {
                num14 = 0f;
            }
            else
            {
                float num15 = num13 * num13;
                num14 = num15 * num15 * Noise.grad(Noise.perm[num9 + num7 + Noise.perm[index + num8]], x4, y2);
            }
            float num16 = (float)(0.5 - (double)x5 * (double)x5 - (double)y3 * (double)y3);
            float num17;
            if ((double)num16 < 0.0)
            {
                num17 = 0f;
            }
            else
            {
                float num18 = num16 * num16;
                num17 = num18 * num18 * Noise.grad(Noise.perm[num9 + 1 + Noise.perm[index + 1]], x5, y3);
            }
            return (float)(40.0 * ((double)num11 + (double)num14 + (double)num17));
        }

        public static float Generate(float x, float y, float z)
        {
            float num1 = (float)(((double)x + (double)y + (double)z) * 0.333333343267441);
            double x1 = (double)x + (double)num1;
            float x2 = y + num1;
            float x3 = z + num1;
            int x4 = Noise.FastFloor((float)x1);
            int x5 = Noise.FastFloor(x2);
            int x6 = Noise.FastFloor(x3);
            float num2 = (x4 + x5 + x6) * 0.1666667f;
            float num3 = x4 - num2;
            float num4 = x5 - num2;
            float num5 = x6 - num2;
            float x7 = x - num3;
            float y1 = y - num4;
            float z1 = z - num5;
            int num6;
            int num7;
            int num8;
            int num9;
            int num10;
            int num11;
            if ((double)x7 >= (double)y1)
            {
                if ((double)y1 >= (double)z1)
                {
                    num6 = 1;
                    num7 = 0;
                    num8 = 0;
                    num9 = 1;
                    num10 = 1;
                    num11 = 0;
                }
                else if ((double)x7 >= (double)z1)
                {
                    num6 = 1;
                    num7 = 0;
                    num8 = 0;
                    num9 = 1;
                    num10 = 0;
                    num11 = 1;
                }
                else
                {
                    num6 = 0;
                    num7 = 0;
                    num8 = 1;
                    num9 = 1;
                    num10 = 0;
                    num11 = 1;
                }
            }
            else if ((double)y1 < (double)z1)
            {
                num6 = 0;
                num7 = 0;
                num8 = 1;
                num9 = 0;
                num10 = 1;
                num11 = 1;
            }
            else if ((double)x7 < (double)z1)
            {
                num6 = 0;
                num7 = 1;
                num8 = 0;
                num9 = 0;
                num10 = 1;
                num11 = 1;
            }
            else
            {
                num6 = 0;
                num7 = 1;
                num8 = 0;
                num9 = 1;
                num10 = 1;
                num11 = 0;
            }
            float x8 = (x7 - num6 + 0.16666667f);
            float y2 = (y1 - num7 + 0.16666667f);
            float z2 = (z1 - num8 + 0.16666667f);
            float x9 = (x7 - num9 + 0.33333334f);
            float y3 = (y1 - num10 + 0.33333334f);
            float z3 = (z1 - num11 + 0.33333334f);
            float x10 = (x7 - 1f + 0.5f);
            float y4 = (y1 - 1f + 0.5f);
            float z4 = (z1 - 1f + 0.5f);
            int num12 = Noise.Mod(x4, 256);
            int num13 = Noise.Mod(x5, 256);
            int index = Noise.Mod(x6, 256);
            float num14 = (0.6f - x7 * x7 - y1 * y1 - z1 * z1);
            float num15;
            if ((double)num14 < 0f)
            {
                num15 = 0f;
            }
            else
            {
                float num16 = num14 * num14;
                num15 = num16 * num16 * Noise.grad(Noise.perm[num12 + Noise.perm[num13 + Noise.perm[index]]], x7, y1, z1);
            }
            float num17 = (0.6f - x8 * x8 - y2 * y2 - z2 * z2);
            float num18;
            if (num17 < 0f)
            {
                num18 = 0f;
            }
            else
            {
                float num19 = num17 * num17;
                num18 = num19 * num19 * Noise.grad(Noise.perm[num12 + num6 + Noise.perm[num13 + num7 + Noise.perm[index + num8]]], x8, y2, z2);
            }
            float num20 = (0.6f - x9 * x9 - y3 * y3 - z3 * z3);
            float num21;
            if ((double)num20 < 0.0)
            {
                num21 = 0f;
            }
            else
            {
                float num22 = num20 * num20;
                num21 = num22 * num22 * Noise.grad(Noise.perm[num12 + num9 + Noise.perm[num13 + num10 + Noise.perm[index + num11]]], x9, y3, z3);
            }
            float num23 = (0.6f - x10 * x10 - y4 * y4 - z4 * z4);
            float num24;
            if ((double)num23 < 0f)
            {
                num24 = 0f;
            }
            else
            {
                float num25 = num23 * num23;
                num24 = num25 * num25 * Noise.grad(Noise.perm[num12 + 1 + Noise.perm[num13 + 1 + Noise.perm[index + 1]]], x10, y4, z4);
            }
            return (32f * (num15 + num18 + num21 + num24));
        }

        public static void Seed(int value)
        {
            Random random = new Random(value);
            for (int index = 0; index < 512; ++index)
                Noise.perm[index] = (byte)Math.Round(random.NextDouble() * byte.MaxValue);
        }

        private static int FastFloor(float x) => (double)x <= 0.0 ? (int)x - 1 : (int)x;

        private static int Mod(int x, int m)
        {
            int num = x % m;
            return num >= 0 ? num : num + m;
        }

        private static float grad(int hash, float x)
        {
            int num1 = hash & 15;
            float num2 = 1f + (num1 & 7);
            if ((num1 & 8) != 0)
                num2 = -num2;
            return num2 * x;
        }

        private static float grad(int hash, float x, float y)
        {
            int num1 = hash & 7;
            float num2 = num1 < 4 ? x : y;
            float num3 = num1 < 4 ? y : x;
            return (((num1 & 1) != 0 ? -num2 : num2) + ((num1 & 2) != 0 ? -2f * num3 : 2f * num3));
        }

        private static float grad(int hash, float x, float y, float z)
        {
            int num1 = hash & 15;
            float num2 = num1 < 8 ? x : y;
            float num3 = num1 < 4 ? y : (num1 == 12 || num1 == 14 ? x : z);
            return (((num1 & 1) != 0 ? -num2 : num2) + ((num1 & 2) != 0 ? -num3 : num3));
        }

        private static float grad(int hash, float x, float y, float z, float t)
        {
            int num1 = hash & 31;
            float num2 = num1 < 24 ? x : y;
            float num3 = num1 < 16 ? y : z;
            float num4 = num1 < 8 ? z : t;
            return (((num1 & 1) != 0 ? -num2 : num2) + ((num1 & 2) != 0 ? -num3 : num3) + ((num1 & 4) != 0 ? -num4 : num4));
        }
    }
}
