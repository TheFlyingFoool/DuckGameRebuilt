// Decompiled with JetBrains decompiler
// Type: DuckGame.StateBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public GhostPriority priority => this._priority;

        public override string ToString() => this.GetDebugString((object)null);

        public virtual string GetDebugString(object with)
        {
            if (with != null)
                return this.name + " = " + Convert.ToString(with);
            return this.classValue == null ? this.name + " = null" : this.name + " = " + Convert.ToString(this.classValue);
        }

        public virtual string GetDebugStringSpecial(object with) => with == null ? this.name + " = null" : this.name + " = " + Convert.ToString(with);

        public bool lerp => this._lerp;

        public bool initialized
        {
            get => this._initialized;
            set => this._initialized = value;
        }

        public string name => this._fieldName;

        public object owner => this._thing;

        public virtual object classValue
        {
            get => this._accessor.getAccessor == null ? (object)null : this._accessor.getAccessor(this._thing);
            set
            {
                if (this._accessor.setAccessor == null)
                    return;
                this._accessor.setAccessor(this._thing, value);
            }
        }

        public virtual T getTyped<T>() => this._accessor.type == typeof(T) ? this._accessor.Get<T>(this._thing) : (T)(object)this.classValue;

        public virtual void setTyped<T>(T value)
        {
            if (this._accessor.type == typeof(T))
                this._accessor.Set<T>(this._thing, value);
            else
                this.classValue = (object)value;
        }

        public virtual byte byteValue
        {
            get => this._accessor.type == typeof(byte) ? this._accessor.Get<byte>(this._thing) : (byte)this.classValue;
            set
            {
                if (this._accessor.type == typeof(byte))
                    this._accessor.Set<byte>(this._thing, value);
                else
                    this.classValue = (object)value;
            }
        }

        public virtual ushort ushortValue
        {
            get => this._accessor.type == typeof(ushort) ? this._accessor.Get<ushort>(this._thing) : (ushort)this.classValue;
            set
            {
                if (this._accessor.type == typeof(ushort))
                    this._accessor.Set<ushort>(this._thing, value);
                else
                    this.classValue = (object)value;
            }
        }

        public virtual int intValue
        {
            get => this._accessor.type == typeof(int) ? this._accessor.Get<int>(this._thing) : (int)this.classValue;
            set
            {
                if (this._accessor.type == typeof(int))
                    this._accessor.Set<int>(this._thing, value);
                else
                    this.classValue = (object)value;
            }
        }

        public virtual System.Type type => this._accessor.type;

        public static bool CompareBase(object o1, object o2)
        {
            switch (o1)
            {
                case float num:
                    return (double)Math.Abs(num - (float)o2) < 1.0 / 1000.0;
                case Vec2 vec2_2:
                    Vec2 vec2_1 = vec2_2 - (Vec2)o2;
                    return (double)Math.Abs(vec2_1.x) < 0.00499999988824129 && (double)Math.Abs(vec2_1.y) < 0.00499999988824129;
                case BitBuffer _:
                    return false;
                default:
                    return object.Equals(o1, o2);
            }
        }

        public bool Compare<T>(T f, out T newVal)
        {
            newVal = (T)(object)this.classValue;
            return StateBinding.CompareBase((object)f, (object)newVal);
        }

        public virtual int bits => this._bits;

        public bool trueOnly => this._trueOnly;

        public bool isRotation => this._isRotation;

        public bool isVelocity => this._isVelocity;

        public StateBinding(string field, int bits = -1, bool rot = false, bool vel = false)
        {
            this._fieldName = field;
            this._previousValue = (object)null;
            this._bits = bits;
            this._isRotation = rot;
            this._isVelocity = vel;
        }

        public StateBinding(bool doLerp, string field, int bits = -1, bool rot = false, bool vel = false)
        {
            this._fieldName = field;
            this._previousValue = (object)null;
            this._bits = bits;
            this._isRotation = rot;
            this._isVelocity = vel;
            this._lerp = doLerp;
            if (!this._lerp)
                return;
            this._priority = GhostPriority.Normal;
        }

        public StateBinding(
          GhostPriority p,
          string field,
          int bits = -1,
          bool rot = false,
          bool vel = false,
          bool doLerp = false)
        {
            this._fieldName = field;
            this._previousValue = (object)null;
            this._bits = bits;
            this._isRotation = rot;
            this._isVelocity = vel;
            this._priority = p;
            this._lerp = doLerp;
        }

        public StateBinding(string field, int bits, bool rot)
        {
            this._fieldName = field;
            this._previousValue = (object)null;
            this._bits = bits;
            this._isRotation = rot;
            this._isVelocity = false;
        }

        public virtual object GetNetValue() => this.classValue;

        public virtual object ReadNetValue(object val) => val;

        public virtual object ReadNetValue(BitBuffer pData) => pData.ReadBits(this.type, this.bits);

        public bool connected => this._accessor != null;

        public virtual void Connect(Thing t)
        {
            this._thing = (object)t;
            this._accessor = Editor.GetAccessorInfo(t.GetType(), this._fieldName);
            if (this._accessor == null)
                throw new Exception("Could not find accessor for binding.");
        }
    }
}
