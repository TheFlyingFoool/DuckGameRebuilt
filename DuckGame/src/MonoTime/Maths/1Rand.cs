// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeRando
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public static class ChallengeRando
    {
        private static Random _randomGenerator;

        public static Random generator
        {
            get => ChallengeRando._randomGenerator;
            set => ChallengeRando._randomGenerator = value;
        }

        public static void DoInitialize() => ChallengeRando._randomGenerator = new Random();

        public static long Long(long min = -9223372036854775808, long max = 9223372036854775807)
        {
            if (ChallengeRando._randomGenerator == null)
                ChallengeRando.DoInitialize();
            byte[] buffer = new byte[8];
            ChallengeRando._randomGenerator.NextBytes(buffer);
            return Math.Abs(BitConverter.ToInt64(buffer, 0) % (max - min)) + min;
        }

        public static double Double() => ChallengeRando._randomGenerator.NextDouble();

        public static float Float(float max) => (float)ChallengeRando._randomGenerator.NextDouble() * max;

        public static float Float(float min, float max) => min + (float)ChallengeRando._randomGenerator.NextDouble() * (max - min);

        public static int Int(int _max) => ChallengeRando._randomGenerator.Next(0, _max + 1);

        public static int Int(int min, int max) => ChallengeRando._randomGenerator.Next(min, max + 1);

        public static int ChooseInt(params int[] _ints) => _ints[Rando.Int(_ints.Length - 1)];

        public static float ChooseFloat(params float[] _ints) => _ints[Rando.Int(_ints.Length)];
    }
}
