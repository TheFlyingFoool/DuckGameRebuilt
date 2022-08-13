// Decompiled with JetBrains decompiler
// Type: DuckGame.Rando
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public static class Rando
    {
        private static Random _randomGenerator;

        public static Random generator
        {
            get => Rando._randomGenerator;
            set => Rando._randomGenerator = value;
        }

        public static void DoInitialize()
        {
            Rando._randomGenerator = new Random();
            ChallengeRando.DoInitialize();
        }

        public static long Long(long min = -9223372036854775808, long max = 9223372036854775807)
        {
            if (Rando._randomGenerator == null)
                Rando.DoInitialize();
            byte[] buffer = new byte[8];
            Rando._randomGenerator.NextBytes(buffer);
            return Math.Abs(BitConverter.ToInt64(buffer, 0) % (max - min)) + min;
        }

        public static double Double() => Rando._randomGenerator.NextDouble();

        public static float Float(float max) => (float)Rando._randomGenerator.NextDouble() * max;

        public static DuckGame.Vec2 Vec2(float minX, float maxX, float minY, float maxY) => new DuckGame.Vec2(Rando.Float(minX, maxX), Rando.Float(minY, maxY));

        public static DuckGame.Vec2 Vec2(DuckGame.Vec2 spanX, DuckGame.Vec2 spanY) => new DuckGame.Vec2(Rando.Float(spanX.x, spanX.y), Rando.Float(spanY.x, spanY.y));

        public static float Float(float min, float max) => min + (float)Rando._randomGenerator.NextDouble() * (max - min);

        public static int Int(int _max) => Rando._randomGenerator.Next(0, _max + 1);

        public static int Int(int min, int max) => Rando._randomGenerator.Next(min, max + 1);

        public static int ChooseInt(params int[] _ints) => _ints[Rando.Int(_ints.Length - 1)];

        public static float ChooseFloat(params float[] _ints) => _ints[Rando.Int(_ints.Length)];

        public static uint UInt()
        {
            byte[] buffer = new byte[4];
            Rando._randomGenerator.NextBytes(buffer);
            uint num = BitConverter.ToUInt32(buffer, 0);
            if (num == 0U)
                num = 1U;
            return num;
        }
    }
}
