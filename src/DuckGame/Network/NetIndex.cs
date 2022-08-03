// Decompiled with JetBrains decompiler
// Type: DuckGame.NetIndex16
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Diagnostics;

namespace DuckGame
{
    [DebuggerDisplay("Index = {_index}")]
    public struct NetIndex16 : IComparable
    {
        public int _index;
        public int max;
        private bool _zeroSpecial;

        public override string ToString() => Convert.ToString(_index);

        public static int MaxForBits(int bits)
        {
            int num = 0;
            for (int index = 0; index < bits; ++index)
                num |= 1 << index;
            return num;
        }

        public NetIndex16(int index = 1, bool zeroSpecial = true)
        {
            _index = index;
            _zeroSpecial = zeroSpecial;
            max = NetIndex16.MaxForBits(16);
            if (_zeroSpecial)
                return;
            ++max;
        }

        public void Increment() => _index = Mod(_index + 1);

        public int Mod(int val) => _zeroSpecial ? Math.Max(val % max, 1) : val % max;

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (obj is NetIndex16 netIndex16)
            {
                if (this < netIndex16)
                    return -1;
                return this > netIndex16 ? 1 : 0;
            }
            int num = (int)obj;
            if (this < num)
                return -1;
            return this > num ? 1 : 0;
        }

        public static int Difference(NetIndex16 c1, NetIndex16 c2)
        {
            int num1 = ((int)c1 - c1.max / 2) % c1.max;
            if (num1 < 0)
                num1 = c1.max + num1;
            int num2 = c1.max - num1;
            return (c1._index + num2) % c1.max - (int)(c2 + num2) % c1.max;
        }

        public static implicit operator NetIndex16(int val) => new NetIndex16(val);

        public static implicit operator int(NetIndex16 val) => val._index;

        public static NetIndex16 operator +(NetIndex16 c1, int c2)
        {
            c1._index = c1.Mod(c1._index + c2);
            return c1;
        }

        public static NetIndex16 operator ++(NetIndex16 c1)
        {
            c1._index = c1.Mod(c1._index + 1);
            return c1;
        }

        public static bool operator <(NetIndex16 c1, NetIndex16 c2)
        {
            int num1 = ((int)c1 - c1.max / 2) % c1.max;
            if (num1 < 0)
                num1 = c1.max + num1;
            int num2 = c1.max - num1;
            return (c1._index + num2) % c1.max < (c2._index + num2) % c1.max;
        }

        public static bool operator >(NetIndex16 c1, NetIndex16 c2)
        {
            if (c1 == 0)
                return false;
            int num1 = ((int)c1 - c1.max / 2) % c1.max;
            if (num1 < 0)
                num1 = c1.max + num1;
            int num2 = c1.max - num1;
            return (c1._index + num2) % c1.max > (c2._index + num2) % c1.max;
        }

        public static bool operator <(NetIndex16 c1, int c2)
        {
            int num1 = ((int)c1 - c1.max / 2) % c1.max;
            if (num1 < 0)
                num1 = c1.max + num1;
            int num2 = c1.max - num1;
            return (c1._index + num2) % c1.max < (c2 + num2) % c1.max;
        }

        public static bool operator >(NetIndex16 c1, int c2) => (int)c1 > c2;

        public static bool operator ==(NetIndex16 c1, NetIndex16 c2) => c1._index == c2._index;

        public static bool operator !=(NetIndex16 c1, NetIndex16 c2) => c1._index != c2._index;

        public static bool operator ==(NetIndex16 c1, int c2) => c1._index == c2;

        public static bool operator !=(NetIndex16 c1, int c2) => c1._index != c2;

        public override bool Equals(object obj) => CompareTo(obj) == 0;

        public override int GetHashCode() => _index.GetHashCode();
    }
}
