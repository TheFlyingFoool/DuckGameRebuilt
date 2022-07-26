// Decompiled with JetBrains decompiler
// Type: DuckGame.NetIndex4
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Diagnostics;

namespace DuckGame
{
    [DebuggerDisplay("Index = {_index}")]
    public struct NetIndex4 : IComparable
    {
        public int _index;
        public int max;
        private bool _zeroSpecial;

        public override string ToString() => this._index.ToString();

        public static int MaxForBits(int bits)
        {
            int num = 0;
            for (int index = 0; index < bits; ++index)
                num |= 1 << index;
            return num;
        }

        public NetIndex4(int index = 1, bool zeroSpecial = false)
        {
            this._index = index;
            this._zeroSpecial = zeroSpecial;
            this.max = NetIndex4.MaxForBits(4);
            if (this._zeroSpecial)
                return;
            ++this.max;
        }

        public void Increment() => this._index = this.Mod(this._index + 1);

        public int Mod(int val) => this._zeroSpecial ? Math.Max(val % this.max, 1) : val % this.max;

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (obj is NetIndex4 netIndex4)
            {
                if (this < netIndex4)
                    return -1;
                return this > netIndex4 ? 1 : 0;
            }
            int num = (int)obj;
            if (this < num)
                return -1;
            return this > num ? 1 : 0;
        }

        public static implicit operator NetIndex4(int val) => new NetIndex4(val);

        public static implicit operator int(NetIndex4 val) => val._index;

        public static NetIndex4 operator +(NetIndex4 c1, int c2)
        {
            c1._index = c1.Mod(c1._index + c2);
            return c1;
        }

        public static NetIndex4 operator ++(NetIndex4 c1)
        {
            c1._index = c1.Mod(c1._index + 1);
            return c1;
        }

        public static bool operator <(NetIndex4 c1, NetIndex4 c2)
        {
            int num1 = ((int)c1 - c1.max / 2) % c1.max;
            if (num1 < 0)
                num1 = c1.max + num1;
            int num2 = c1.max - num1;
            return (c1._index + num2) % c1.max < (c2._index + num2) % c1.max;
        }

        public static bool operator >(NetIndex4 c1, NetIndex4 c2) => (int)c1 > (int)c2;

        public static bool operator <(NetIndex4 c1, int c2)
        {
            int num1 = ((int)c1 - c1.max / 2) % c1.max;
            if (num1 < 0)
                num1 = c1.max + num1;
            int num2 = c1.max - num1;
            return (c1._index + num2) % c1.max < (c2 + num2) % c1.max;
        }

        public static bool operator >(NetIndex4 c1, int c2) => (int)c1 > c2;

        public static bool operator ==(NetIndex4 c1, NetIndex4 c2) => c1._index == c2._index;

        public static bool operator !=(NetIndex4 c1, NetIndex4 c2) => c1._index != c2._index;

        public static bool operator ==(NetIndex4 c1, int c2) => c1._index == c2;

        public static bool operator !=(NetIndex4 c1, int c2) => c1._index != c2;

        public override bool Equals(object obj) => this.CompareTo(obj) == 0;

        public override int GetHashCode() => this._index.GetHashCode();
    }
}
