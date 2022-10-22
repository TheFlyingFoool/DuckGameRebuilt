// Decompiled with JetBrains decompiler
// Type: DuckGame.NetRand
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public static class NetRand
    {
        private static Random _randomGenerator;
        public static int currentSeed;

        public static Random generator
        {
            get => NetRand._randomGenerator;
            set => NetRand._randomGenerator = value;
        }

        public static void Initialize(int seed)
        {
            NetRand.currentSeed = seed;
            NetRand._randomGenerator = new Random(seed);
        }

        public static void Initialize()
        {
            NetRand._randomGenerator = new Random();
            NetRand.currentSeed = Rando.Int(2147483646);
        }

        public static double Double() => NetRand._randomGenerator.NextDouble();

        public static float Float(float max) => (float)NetRand._randomGenerator.NextDouble() * max;

        public static float Float(float min, float max) => min + (float)NetRand._randomGenerator.NextDouble() * (max - min);

        public static int Int(int _max) => NetRand._randomGenerator.Next(0, _max + 1);

        public static int Int(int min, int max) => NetRand._randomGenerator.Next(min, max + 1);

        public static int ChooseInt(params int[] _ints) => _ints[Rando.Int(_ints.Length - 1)];

        public static float ChooseFloat(params float[] _ints) => _ints[Rando.Int(_ints.Length)];
    }
}
