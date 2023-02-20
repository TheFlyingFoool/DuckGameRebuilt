// Decompiled with JetBrains decompiler
// Type: DuckGame.BufferedGhostProperty
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public override string ToString() => isNetworkStateValue ? binding.GetDebugStringSpecial(value) + "(ns)" : binding.GetDebugStringSpecial(value);

        public abstract object value { get; set; }

        public abstract bool Refresh();

        public abstract void Apply(float lerp);

        public abstract void UpdateFrom(StateBinding bind);

        public void UpdateFrom(BufferedGhostProperty prop)
        {
            tick = prop.tick;
            UpdateFrom(prop.binding);
        }

        protected static Vec2 Slerp(Vec2 from, Vec2 to, float step)
        {
            if (step == 0f)
                return from;
            if (from == to || step == 1f)
                return to;
            double a = Math.Acos(Vec2.Dot(from, to));
            if (a == 0f)
                return to;
            double num = Math.Sin(a);
            return (float)(Math.Sin((1f - step) * a) / num) * from + (float)(Math.Sin(step * a) / num) * to;
        }
    }
}
