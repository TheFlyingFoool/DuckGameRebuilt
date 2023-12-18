using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class StateFlagBinding : StateBinding
    {
        private string[] _fields;
        private List<AccessorInfo> _accessors = new List<AccessorInfo>();
        private ushort _value;

        public override string ToString() => GetDebugString(null);

        public override string GetDebugString(object with)
        {
            string debugString = "";
            int index = 0;
            byte num = 1;
            foreach (string field in _fields)
            {
                if (with == null)
                    debugString = debugString + field + ": " + Convert.ToString((bool)_accessors[index].getAccessor(_thing) ? 1 : 0) + " | ";
                else
                    debugString = debugString + field + ": " + Convert.ToString(((ushort)with & 1L << _bits - num) != 0L ? 1 : 0) + " | ";
                ++index;
                ++num;
            }
            return debugString;
        }

        public override Type type => typeof(ushort);

        public override object classValue
        {
            get
            {
                _value = 0;
                bool flag = true;
                foreach (AccessorInfo accessor in _accessors)
                {
                    if (!flag)
                        _value <<= 1;
                    _value |= (bool)accessor.getAccessor(_thing) ? (ushort)1 : (ushort)0;
                    flag = false;
                }
                return _value;
            }
            set
            {
                _value = (ushort)value;
                byte num = 1;
                foreach (AccessorInfo accessor in _accessors)
                {
                    accessor.setAccessor(_thing, (_value & (ulong)(1L << _bits - num)) > 0UL);
                    ++num;
                }
            }
        }

        public bool Contains(string pKey)
        {
            foreach (string field in _fields)
            {
                if (field == pKey)
                    return true;
            }
            return false;
        }

        public bool Value(string pKey, ushort pValue)
        {
            byte num = 1;
            foreach (string field in _fields)
            {
                if (field == pKey)
                    return (pValue & (ulong)(1L << _bits - num)) > 0UL;
                ++num;
            }
            return false;
        }

        public StateFlagBinding(params string[] fields)
          : base("multiple")
        {
            _fields = fields;
            _priority = GhostPriority.Normal;
        }

        public StateFlagBinding(GhostPriority p, params string[] fields)
          : base("multiple")
        {
            _fields = fields;
            _priority = p;
        }

        public override void Connect(Thing t)
        {
            _bits = 0;
            _thing = t;
            Type type = t.GetType();
            _accessors.Clear();
            foreach (string field in _fields)
            {
                _accessors.Add(Editor.GetAccessorInfo(type, field));
                ++_bits;
            }
        }
    }
}
