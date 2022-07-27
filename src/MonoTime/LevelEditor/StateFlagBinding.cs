// Decompiled with JetBrains decompiler
// Type: DuckGame.StateFlagBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class StateFlagBinding : StateBinding
    {
        private string[] _fields;
        private List<AccessorInfo> _accessors = new List<AccessorInfo>();
        private ushort _value;

        public override string ToString() => this.GetDebugString(null);

        public override string GetDebugString(object with)
        {
            string debugString = "";
            int index = 0;
            byte num = 1;
            foreach (string field in this._fields)
            {
                if (with == null)
                    debugString = debugString + field + ": " + Convert.ToString((bool)this._accessors[index].getAccessor(this._thing) ? 1 : 0) + " | ";
                else
                    debugString = debugString + field + ": " + Convert.ToString(((ushort)with & 1L << this._bits - num) != 0L ? 1 : 0) + " | ";
                ++index;
                ++num;
            }
            return debugString;
        }

        public override System.Type type => typeof(ushort);

        public override object classValue
        {
            get
            {
                this._value = 0;
                bool flag = true;
                foreach (AccessorInfo accessor in this._accessors)
                {
                    if (!flag)
                        this._value <<= 1;
                    this._value |= (bool)accessor.getAccessor(this._thing) ? (ushort)1 : (ushort)0;
                    flag = false;
                }
                return _value;
            }
            set
            {
                this._value = (ushort)value;
                byte num = 1;
                foreach (AccessorInfo accessor in this._accessors)
                {
                    accessor.setAccessor(this._thing, (_value & (ulong)(1L << this._bits - num)) > 0UL);
                    ++num;
                }
            }
        }

        public bool Contains(string pKey)
        {
            foreach (string field in this._fields)
            {
                if (field == pKey)
                    return true;
            }
            return false;
        }

        public bool Value(string pKey, ushort pValue)
        {
            byte num = 1;
            foreach (string field in this._fields)
            {
                if (field == pKey)
                    return (pValue & (ulong)(1L << this._bits - num)) > 0UL;
                ++num;
            }
            return false;
        }

        public StateFlagBinding(params string[] fields)
          : base("multiple")
        {
            this._fields = fields;
            this._priority = GhostPriority.Normal;
        }

        public StateFlagBinding(GhostPriority p, params string[] fields)
          : base("multiple")
        {
            this._fields = fields;
            this._priority = p;
        }

        public override void Connect(Thing t)
        {
            this._bits = 0;
            this._thing = t;
            System.Type type = t.GetType();
            this._accessors.Clear();
            foreach (string field in this._fields)
            {
                this._accessors.Add(Editor.GetAccessorInfo(type, field));
                ++this._bits;
            }
        }
    }
}
