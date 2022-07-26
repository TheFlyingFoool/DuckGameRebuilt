// Decompiled with JetBrains decompiler
// Type: DuckGame.BufferedGhostProperty
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public abstract class BufferedGhostProperty
    {
        public StateBinding binding;
        public int index;
        public NetIndex16 tick;
        public bool isNetworkStateValue;
        public bool initialized;

        public override string ToString() => this.isNetworkStateValue ? this.binding.GetDebugStringSpecial(this.value) + "(ns)" : this.binding.GetDebugStringSpecial(this.value);

        public abstract object value { get; set; }

        public abstract bool Refresh();

        public abstract void Apply(float lerp);

        public abstract void UpdateFrom(StateBinding bind);

        public void UpdateFrom(BufferedGhostProperty prop)
        {
            this.tick = prop.tick;
            this.UpdateFrom(prop.binding);
        }

        protected static Vec2 Slerp(Vec2 from, Vec2 to, float step)
        {
            if ((double)step == 0.0)
                return from;
            if (from == to || (double)step == 1.0)
                return to;
            double a = Math.Acos((double)Vec2.Dot(from, to));
            if (a == 0.0)
                return to;
            double num = Math.Sin(a);
            return (float)(Math.Sin((1.0 - (double)step) * a) / num) * from + (float)(Math.Sin((double)step * a) / num) * to;
        }
    }
}
