using System;
using System.Diagnostics;

namespace DuckGame
{
    [DebuggerDisplay("Index = {_index}")]
    public struct NetIndex8 : IComparable
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

        public NetIndex8(int index = 1, bool zeroSpecial = true)
        {
            _index = index;
            _zeroSpecial = false;
            max = MaxForBits(8);
        }

        public void Increment() => _index = Mod(_index + 1);

        public int Mod(int val) => _zeroSpecial ? Math.Max(val % max, 1) : val % max;

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (obj is NetIndex8 netIndex8)
            {
                if (this < netIndex8)
                    return -1;
                return this > netIndex8 ? 1 : 0;
            }
            int num = (int)obj;
            if (this < num)
                return -1;
            return this > num ? 1 : 0;
        }

        public static implicit operator NetIndex8(int val) => new NetIndex8(val);

        public static implicit operator int(NetIndex8 val) => val._index;

        public static NetIndex8 operator +(NetIndex8 c1, int c2)
        {
            c1._index = c1.Mod(c1._index + c2);
            return c1;
        }

        public static NetIndex8 operator ++(NetIndex8 c1)
        {
            c1._index = c1.Mod(c1._index + 1);
            return c1;
        }

        public static bool operator <(NetIndex8 c1, NetIndex8 c2)
        {
            int num1 = ((int)c1 - c1.max / 2) % c1.max;
            if (num1 < 0)
                num1 = c1.max + num1;
            int num2 = c1.max - num1;
            return (c1._index + num2) % c1.max < (c2._index + num2) % c1.max;
        }

        public static bool operator >(NetIndex8 c1, NetIndex8 c2)
        {
            int num1 = ((int)c1 - c1.max / 2) % c1.max;
            if (num1 < 0)
                num1 = c1.max + num1;
            int num2 = c1.max - num1;
            return (c1._index + num2) % c1.max > (c2._index + num2) % c1.max;
        }

        public static bool operator <(NetIndex8 c1, int c2)
        {
            int num1 = ((int)c1 - c1.max / 2) % c1.max;
            if (num1 < 0)
                num1 = c1.max + num1;
            int num2 = c1.max - num1;
            return (c1._index + num2) % c1.max < (c2 + num2) % c1.max;
        }

        public static int Difference(NetIndex8 c1, NetIndex8 c2)
        {
            int num1 = ((int)c1 - c1.max / 2) % c1.max;
            if (num1 < 0)
                num1 = c1.max + num1;
            int num2 = c1.max - num1;
            return Math.Abs((c1._index + num2) % c1.max - (int)(c2 + num2) % c1.max);
        }

        public static bool operator >(NetIndex8 c1, int c2) => (int)c1 > c2;

        public static bool operator ==(NetIndex8 c1, NetIndex8 c2) => c1._index == c2._index;

        public static bool operator !=(NetIndex8 c1, NetIndex8 c2) => c1._index != c2._index;

        public static bool operator ==(NetIndex8 c1, int c2) => c1._index == c2;

        public static bool operator !=(NetIndex8 c1, int c2) => c1._index != c2;

        public override bool Equals(object obj) => CompareTo(obj) == 0;

        public override int GetHashCode() => _index.GetHashCode();
    }
}
