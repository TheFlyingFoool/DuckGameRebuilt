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
            get => this._value;
            set => this._value = value;
        }

        public int valueInt
        {
            get => this._value is float ? (int)(float)this._value : (int)this._value;
            set
            {
                if (!(this._value is int) || value > (int)this._value)
                    this._value = (object)value;
                Steam.SetStat(this._name, this.valueInt);
            }
        }

        public float valueFloat
        {
            get => this._value is int ? (float)(int)this._value : (float)this._value;
            set
            {
                if (!(this._value is float) || (double)value > (double)(float)this._value)
                    this._value = (object)value;
                Steam.SetStat(this._name, this.valueFloat);
            }
        }

        public void BindName(string name)
        {
            this._name = name;
            this._value = (object)0.0f;
            if (!Steam.IsInitialized())
                return;
            float stat = Steam.GetStat(this._name);
            if ((double)stat <= -99999.0)
                return;
            this._value = (object)stat;
        }

        public bool isFloat => this._value is float;

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

        public static bool operator <(StatBinding c1, float c2) => (double)c1.valueFloat < (double)c2;

        public static bool operator >(StatBinding c1, float c2) => (double)c1.valueFloat > (double)c2;

        public static bool operator <(StatBinding c1, int c2) => c1.valueInt < c2;

        public static bool operator >(StatBinding c1, int c2) => c1.valueInt > c2;
    }
}
