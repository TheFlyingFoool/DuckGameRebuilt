using System;

namespace DuckGame
{
    public struct Animation : IEquatable<Animation>
    {
        public string name;
        public float speed;
        public int[] frames;
        public bool looping;

        public Animation(string nameVal, float speedVal, bool loopVal, int[] framesVal)
        {
            name = nameVal;
            speed = speedVal;
            frames = framesVal;
            looping = loopVal;
        }

        public static bool operator ==(Animation l, Animation r) => l.Equals(r);

        public static bool operator !=(Animation l, Animation r) => !l.Equals(r);

        public bool Equals(Animation other) => name == other.name && speed == other.speed && frames == other.frames && looping == other.looping;

        public override int GetHashCode() => ((name.GetHashCode() * 19 + speed.GetHashCode()) * 19 + frames.GetHashCode()) * 19 + looping.GetHashCode();

        public override bool Equals(object obj) => obj is Animation other ? Equals(other) : base.Equals(obj);
    }
}
