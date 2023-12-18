namespace DuckGame
{
    public struct Depth
    {
        public static float kDepthSpanMax = 0.01f;
        public static float kSpanIncrement = 0.0001f;
        private static float _currentSpan = 0f;
        public float value;
        public float span;

        public static void ResetSpan() => _currentSpan = 0f;

        public Depth(float val)
        {
            value = val;
            span = _currentSpan + kSpanIncrement * 0.5f;
            _currentSpan = (_currentSpan + kSpanIncrement) % kDepthSpanMax;
        }

        public Depth(float val, float s)
        {
            value = val;
            span = s;
        }

        public Depth Add(int val)
        {
            // return ((int)((this.value + Depth.kSpanIncrement / 20f * (float)val) * 1000)) / 1000f; //idk man fck floating points
            return new Depth(value + kSpanIncrement / 20f * val, span);
        }

        public static implicit operator Depth(float val) => new Depth(val);

        public static Depth operator +(Depth c1, int c2) => c1.Add(c2);

        public static Depth operator -(Depth c1, int c2) => c1.Add(-c2);

        public static bool operator <(Depth c1, float c2) => c1.value < c2;

        public static bool operator >(Depth c1, float c2) => c1.value > c2;
    }
}
