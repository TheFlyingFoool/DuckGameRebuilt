// Decompiled with JetBrains decompiler
// Type: DuckGame.Animation
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
