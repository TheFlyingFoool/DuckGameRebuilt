// Decompiled with JetBrains decompiler
// Type: DuckGame.StatBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class StatBinding
    {
        private string _name;
        private object _value;

        public object value
        {
            get => _value;
            set => _value = value;
        }

        public int valueInt
        {
            get => _value is float ? (int)(float)_value : (int)_value;
            set
            {
                if (!(_value is int) || value > (int)_value)
                    _value = value;
                Steam.SetStat(_name, valueInt);
            }
        }

        public float valueFloat
        {
            get => _value is int ? (int)_value : (float)_value;
            set
            {
                if (!(_value is float) || value > (float)_value)
                    _value = value;
                Steam.SetStat(_name, valueFloat);
            }
        }

        public void BindName(string name)
        {
            _name = name;
            _value = 0f;
            if (!Steam.IsInitialized())
                return;
            float stat = Steam.GetStat(_name);
            if (stat <= -99999f)
                return;
            _value = stat;
        }

        public bool isFloat => _value is float;

        public static implicit operator float(StatBinding val) => val.valueFloat;

        public static implicit operator int(StatBinding val) => val.valueInt;

        public static StatBinding operator +(StatBinding c1, int c2)
        {
            c1.valueInt += c2;
            return c1;
        }

        public static StatBinding operator -(StatBinding c1, int c2)
        {
            c1.valueInt -= c2;
            return c1;
        }

        public static StatBinding operator +(StatBinding c1, float c2)
        {
            c1.valueFloat += c2;
            return c1;
        }

        public static StatBinding operator -(StatBinding c1, float c2)
        {
            c1.valueFloat -= c2;
            return c1;
        }

        public static bool operator <(StatBinding c1, float c2) => c1.valueFloat < c2;

        public static bool operator >(StatBinding c1, float c2) => c1.valueFloat > c2;

        public static bool operator <(StatBinding c1, int c2) => c1.valueInt < c2;

        public static bool operator >(StatBinding c1, int c2) => c1.valueInt > c2;
    }
}
