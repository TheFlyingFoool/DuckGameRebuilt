using System;

namespace DuckGame
{
    public abstract class FFUITransition
    {
        public float Direction = 0;
        public Func<ProgressValue, ProgressValue> EasingFunction;
        public FFMainWindow MainWindow;
        public float TransitionProgress;
        public bool Finished => Math.Abs(TransitionProgress - 1f) < 0.0001f;

        public abstract void Start();
        public abstract void Update();
        public abstract void End();

        // using my own functions cus Vec2.Lerp, Lerp.FloatSmooth, and all others suck ass
        protected static float lerp(float a, float b, float t) => a + (b - a) * t;
        protected static Vec2 lerp(Vec2 a, Vec2 b, float t) => new Vec2(lerp(a.x, b.x, t), lerp(a.y, b.y, t));
    }
}