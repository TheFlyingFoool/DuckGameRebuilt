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
        /// <summary>
        /// 1D simplex noise
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Generate(float x)
        {
            int i0 = Noise.FastFloor(x);
            int i = i0 + 1;
            float x2 = x - i0;
            float x3 = x2 - 1f;
            float num = 1f - x2 * x2;
            float num2 = num * num;
            float n0 = num2 * num2 * Noise.grad(Noise.perm[i0 & 255], x2);
            float num3 = 1f - x3 * x3;
            float num4 = num3 * num3;
            float n = num4 * num4 * Noise.grad(Noise.perm[i & 255], x3);
            return 0.395f * (n0 + n);
        }

        /// <summary>
        /// 2D simplex noise
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float Generate(float x, float y)
        {
            float s = (x + y) * 0.3660254f;
            float x5 = x + s;
            float ys = y + s;
            int num = Noise.FastFloor(x5);
            int i = Noise.FastFloor(ys);
            float t = (num + i) * 0.21132487f;
            float X0 = num - t;
            float Y0 = i - t;
            float x2 = x - X0;
            float y2 = y - Y0;
            int i2;
            int j;
            if (x2 > y2)
            {
                i2 = 1;
                j = 0;
            }
            else
            {
                i2 = 0;
                j = 1;
            }
            float x3 = x2 - i2 + 0.21132487f;
            float y3 = y2 - j + 0.21132487f;
            float x4 = x2 - 1f + 0.42264974f;
            float y4 = y2 - 1f + 0.42264974f;
            int ii = num % 256;
            int jj = i % 256;
            float t2 = 0.5f - x2 * x2 - y2 * y2;
            float n0;
            if (t2 < 0f)
            {
                n0 = 0f;
            }
            else
            {
                t2 *= t2;
                n0 = t2 * t2 * Noise.grad(Noise.perm[ii + Noise.perm[jj]], x2, y2);
            }
            float t3 = 0.5f - x3 * x3 - y3 * y3;
            float n;
            if (t3 < 0f)
            {
                n = 0f;
            }
            else
            {
                t3 *= t3;
                n = t3 * t3 * Noise.grad(Noise.perm[ii + i2 + Noise.perm[jj + j]], x3, y3);
            }
            float t4 = 0.5f - x4 * x4 - y4 * y4;
            float n2;
            if (t4 < 0f)
            {
                n2 = 0f;
            }
            else
            {
                t4 *= t4;
                n2 = t4 * t4 * Noise.grad(Noise.perm[ii + 1 + Noise.perm[jj + 1]], x4, y4);
            }
            return 40f * (n0 + n + n2);
        }

        public static float Generate(float x, float y, float z)
        {
            float s = (x + y + z) * 0.33333334f;
            float x6 = x + s;
            float ys = y + s;
            float zs = z + s;
            int num = Noise.FastFloor(x6);
            int i = Noise.FastFloor(ys);
            int j = Noise.FastFloor(zs);
            float t = (num + i + j) * 0.16666667f;
            float X0 = num - t;
            float Y0 = i - t;
            float Z0 = j - t;
            float x2 = x - X0;
            float y2 = y - Y0;
            float z2 = z - Z0;
            int i2;
            int j2;
            int k;
            int i3;
            int j3;
            int k2;
            if (x2 >= y2)
            {
                if (y2 >= z2)
                {
                    i2 = 1;
                    j2 = 0;
                    k = 0;
                    i3 = 1;
                    j3 = 1;
                    k2 = 0;
                }
                else if (x2 >= z2)
                {
                    i2 = 1;
                    j2 = 0;
                    k = 0;
                    i3 = 1;
                    j3 = 0;
                    k2 = 1;
                }
                else
                {
                    i2 = 0;
                    j2 = 0;
                    k = 1;
                    i3 = 1;
                    j3 = 0;
                    k2 = 1;
                }
            }
            else if (y2 < z2)
            {
                i2 = 0;
                j2 = 0;
                k = 1;
                i3 = 0;
                j3 = 1;
                k2 = 1;
            }
            else if (x2 < z2)
            {
                i2 = 0;
                j2 = 1;
                k = 0;
                i3 = 0;
                j3 = 1;
                k2 = 1;
            }
            else
            {
                i2 = 0;
                j2 = 1;
                k = 0;
                i3 = 1;
                j3 = 1;
                k2 = 0;
            }
            float x3 = x2 - i2 + 0.16666667f;
            float y3 = y2 - j2 + 0.16666667f;
            float z3 = z2 - k + 0.16666667f;
            float x4 = x2 - i3 + 0.33333334f;
            float y4 = y2 - j3 + 0.33333334f;
            float z4 = z2 - k2 + 0.33333334f;
            float x5 = x2 - 1f + 0.5f;
            float y5 = y2 - 1f + 0.5f;
            float z5 = z2 - 1f + 0.5f;
            int ii = Noise.Mod(num, 256);
            int jj = Noise.Mod(i, 256);
            int kk = Noise.Mod(j, 256);
            float t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            float n0;
            if (t2 < 0f)
            {
                n0 = 0f;
            }
            else
            {
                t2 *= t2;
                n0 = t2 * t2 * Noise.grad(Noise.perm[ii + Noise.perm[jj + Noise.perm[kk]]], x2, y2, z2);
            }
            float t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            float n;
            if (t3 < 0f)
            {
                n = 0f;
            }
            else
            {
                t3 *= t3;
                n = t3 * t3 * Noise.grad(Noise.perm[ii + i2 + Noise.perm[jj + j2 + Noise.perm[kk + k]]], x3, y3, z3);
            }
            float t4 = 0.6f - x4 * x4 - y4 * y4 - z4 * z4;
            float n2;
            if (t4 < 0f)
            {
                n2 = 0f;
            }
            else
            {
                t4 *= t4;
                n2 = t4 * t4 * Noise.grad(Noise.perm[ii + i3 + Noise.perm[jj + j3 + Noise.perm[kk + k2]]], x4, y4, z4);
            }
            float t5 = 0.6f - x5 * x5 - y5 * y5 - z5 * z5;
            float n3;
            if (t5 < 0f)
            {
                n3 = 0f;
            }
            else
            {
                t5 *= t5;
                n3 = t5 * t5 * Noise.grad(Noise.perm[ii + 1 + Noise.perm[jj + 1 + Noise.perm[kk + 1]]], x5, y5, z5);
            }
            return 32f * (n0 + n + n2 + n3);
        }

        public static void Seed(int value)
        {
            Random r = new Random(value);
            for (int i = 0; i < 512; i++)
            {
                Noise.perm[i] = (byte)Math.Round(r.NextDouble() * 255.0);
            }
        }

        private static int FastFloor(float x)
        {
            if (x <= 0f)
            {
                return (int)x - 1;
            }
            return (int)x;
        }

        private static int Mod(int x, int m)
        {
            int a = x % m;
            if (a >= 0)
            {
                return a;
            }
            return a + m;
        }

        private static float grad(int hash, float x)
        {
            int h = hash & 15;
            float grad = 1f + (h & 7);
            if ((h & 8) != 0)
            {
                grad = -grad;
            }
            return grad * x;
        }

        private static float grad(int hash, float x, float y)
        {
            int h = hash & 7;
            float u = (h < 4) ? x : y;
            float v = (h < 4) ? y : x;
            return (((h & 1) != 0) ? (-u) : u) + (((h & 2) != 0) ? (-2f * v) : (2f * v));
        }

        private static float grad(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = (h < 8) ? x : y;
            float v = (h < 4) ? y : ((h == 12 || h == 14) ? x : z);
            return (((h & 1) != 0) ? (-u) : u) + (((h & 2) != 0) ? (-v) : v);
        }

        private static float grad(int hash, float x, float y, float z, float t)
        {
            int h = hash & 31;
            float u = (h < 24) ? x : y;
            float v = (h < 16) ? y : z;
            float w = (h < 8) ? z : t;
            return (((h & 1) != 0) ? (-u) : u) + (((h & 2) != 0) ? (-v) : v) + (((h & 4) != 0) ? (-w) : w);
        }

        public Noise()
        {
        }

        // Note: this type is marked as 'beforefieldinit'.
        static Noise()
        {
        }

        public static byte[] perm = new byte[]
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
    }
}
