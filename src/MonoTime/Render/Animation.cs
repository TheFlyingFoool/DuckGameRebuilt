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
            this.name = nameVal;
            this.speed = speedVal;
            this.frames = framesVal;
            this.looping = loopVal;
        }

        public static bool operator ==(Animation l, Animation r) => l.Equals(r);

        public static bool operator !=(Animation l, Animation r) => !l.Equals(r);

        public bool Equals(Animation other) => this.name == other.name && speed == other.speed && this.frames == other.frames && this.looping == other.looping;

        public override int GetHashCode() => ((this.name.GetHashCode() * 19 + this.speed.GetHashCode()) * 19 + this.frames.GetHashCode()) * 19 + this.looping.GetHashCode();

        public override bool Equals(object obj) => obj is Animation other ? this.Equals(other) : base.Equals(obj);
    }
}
