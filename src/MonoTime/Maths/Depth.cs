// Decompiled with JetBrains decompiler
// Type: DuckGame.Depth
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public struct Depth
    {
        public static float kDepthSpanMax = 0.01f;
        public static float kSpanIncrement = 0.0001f;
        private static float _currentSpan = 0.0f;
        public float value;
        public float span;

        public static void ResetSpan() => Depth._currentSpan = 0.0f;

        public Depth(float val)
        {
            this.value = val;
            this.span = Depth._currentSpan + Depth.kSpanIncrement * 0.5f;
            Depth._currentSpan = (Depth._currentSpan + Depth.kSpanIncrement) % Depth.kDepthSpanMax;
        }

        public Depth(float val, float s)
        {
            this.value = val;
            this.span = s;
        }

        public Depth Add(int val) => new Depth(this.value + Depth.kSpanIncrement / 20f * val, this.span);

        public static implicit operator Depth(float val) => new Depth(val);

        public static Depth operator +(Depth c1, int c2) => c1.Add(c2);

        public static Depth operator -(Depth c1, int c2) => c1.Add(-c2);

        public static bool operator <(Depth c1, float c2) => c1.value < (double)c2;

        public static bool operator >(Depth c1, float c2) => c1.value > (double)c2;
    }
}
