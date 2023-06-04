// Decompiled with JetBrains decompiler
// Type: DuckGame.StateBinding
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    /// <summary>
    /// A state binding allows a Thing to communicate the state of a field over the network during multiplayer.
    /// These are generally private members of your Thing.
    /// </summary>
    public class StateBinding
    {
        public int bitIndex;
        protected GhostPriority _priority;
        public bool valid = true;
        protected bool _lerp;
        private bool _initialized = true;
        private string _fieldName;
        protected object _thing;
        public object _previousValue;
        protected int _bits = -1;
        private bool _trueOnly;
        private bool _isRotation;
        private bool _isVelocity;
        public bool skipLerp;
        protected AccessorInfo _accessor = new AccessorInfo();

        public GhostPriority priority => _priority;

        public override string ToString() => GetDebugString(null);

        public virtual string GetDebugString(object with)
        {
            if (with != null)
                return name + " = " + Convert.ToString(with);
            return classValue == null ? name + " = null" : name + " = " + Convert.ToString(classValue);
        }

        public virtual string GetDebugStringSpecial(object with) => with == null ? name + " = null" : name + " = " + Convert.ToString(with);

        public bool lerp => _lerp;

        public bool initialized
        {
            get => _initialized;
            set => _initialized = value;
        }

        public string name => _fieldName;

        public object owner => _thing;

        public virtual object classValue
        {
            get => _accessor.getAccessor == null ? null : _accessor.getAccessor(_thing);
            set
            {
                if (_accessor.setAccessor == null)
                    return;
                _accessor.setAccessor(_thing, value);
            }
        }

        public virtual T getTyped<T>() => _accessor.type == typeof(T) ? _accessor.Get<T>(_thing) : (T)classValue;

        public virtual void setTyped<T>(T value)
        {
            if (_accessor.type == typeof(T))
                _accessor.Set(_thing, value);
            else
                classValue = value;
        }

        public virtual byte byteValue
        {
            get => _accessor.type == typeof(byte) ? _accessor.Get<byte>(_thing) : (byte)classValue;
            set
            {
                if (_accessor.type == typeof(byte))
                    _accessor.Set(_thing, value);
                else
                    classValue = value;
            }
        }

        public virtual ushort ushortValue
        {
            get => _accessor.type == typeof(ushort) ? _accessor.Get<ushort>(_thing) : (ushort)classValue;
            set
            {
                if (_accessor.type == typeof(ushort))
                    _accessor.Set(_thing, value);
                else
                    classValue = value;
            }
        }

        public virtual int intValue
        {
            get => _accessor.type == typeof(int) ? _accessor.Get<int>(_thing) : (int)classValue;
            set
            {
                if (_accessor.type == typeof(int))
                    _accessor.Set(_thing, value);
                else
                    classValue = value;
            }
        }

        public virtual Type type => _accessor.type;

        public static bool CompareBase(object o1, object o2)
        {
            switch (o1)
            {
                case float num:
                    return Math.Abs(num - (float)o2) < 1f / 1000f;
                case Vec2 vec2_2:
                    Vec2 vec2_1 = vec2_2 - (Vec2)o2;
                    return Math.Abs(vec2_1.x) < 0.005f && Math.Abs(vec2_1.y) < 0.005f;
                case BitBuffer _:
                    return false;
                default:
                    return Equals(o1, o2);
            }
        }

        public bool Compare<T>(T f, out T newVal)
        {
            newVal = (T)classValue;
            return CompareBase(f, newVal);
        }

        public virtual int bits => _bits;

        public bool trueOnly => _trueOnly;

        public bool isRotation => _isRotation;

        public bool isVelocity => _isVelocity;

        public StateBinding(string field, int bits = -1, bool rot = false, bool vel = false)
        {
            _fieldName = field;
            _previousValue = null;
            _bits = bits;
            _isRotation = rot;
            _isVelocity = vel;
        }

        public StateBinding(bool doLerp, string field, int bits = -1, bool rot = false, bool vel = false)
        {
            _fieldName = field;
            _previousValue = null;
            _bits = bits;
            _isRotation = rot;
            _isVelocity = vel;
            _lerp = doLerp;
            if (!_lerp)
                return;
            _priority = GhostPriority.Normal;
        }

        public StateBinding(
          GhostPriority p,
          string field,
          int bits = -1,
          bool rot = false,
          bool vel = false,
          bool doLerp = false)
        {
            _fieldName = field;
            _previousValue = null;
            _bits = bits;
            _isRotation = rot;
            _isVelocity = vel;
            _priority = p;
            _lerp = doLerp;
        }

        public StateBinding(string field, int bits, bool rot)
        {
            _fieldName = field;
            _previousValue = null;
            _bits = bits;
            _isRotation = rot;
            _isVelocity = false;
        }

        public virtual object GetNetValue() => classValue;

        public virtual object ReadNetValue(object val) => val;

        public virtual object ReadNetValue(BitBuffer pData) => pData.ReadBits(type, bits);

        public bool connected => _accessor != null;

        public virtual void Connect(Thing t)
        {
            _thing = t;
            _accessor = Editor.GetAccessorInfo(t.GetType(), _fieldName);
            if (_accessor == null)
                throw new Exception("Could not find accessor for binding.");
        }
    }
}
