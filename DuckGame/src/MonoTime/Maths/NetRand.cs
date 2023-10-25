using System;

namespace DuckGame
{
    public static class NetRand
    {
        private static Random _randomGenerator;
        public static int currentSeed;

        public static Random generator
        {
            get => _randomGenerator;
            set => _randomGenerator = value;
        }

        public static void Initialize(int seed)
        {
            currentSeed = seed;
            _randomGenerator = new Random(seed);
        }

        public static void Initialize()
        {
            _randomGenerator = new Random();
            currentSeed = Rando.Int(2147483646);
        }

        public static double Double() => _randomGenerator.NextDouble();

        public static float Float(float max) => (float)_randomGenerator.NextDouble() * max;

        public static float Float(float min, float max) => min + (float)_randomGenerator.NextDouble() * (max - min);

        public static int Int(int _max) => _randomGenerator.Next(0, _max + 1);

        public static int Int(int min, int max) => _randomGenerator.Next(min, max + 1);

        public static int ChooseInt(params int[] _ints) => _ints[Rando.Int(_ints.Length - 1)];

        public static float ChooseFloat(params float[] _ints) => _ints[Rando.Int(_ints.Length)];
    }
}
