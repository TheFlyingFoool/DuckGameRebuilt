// Decompiled with JetBrains decompiler
// Type: DuckGame.FieldBinding
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Reflection;

namespace DuckGame
{
    public class FieldBinding
    {
        protected object _thing;
        protected FieldInfo _field;
        protected PropertyInfo _property;
        private float _min;
        private float _max;
        private float _inc;

        public object thing => this._thing;

        public virtual object value
        {
            get => !(this._field != null) ? this._property.GetValue(this._thing, null) : this._field.GetValue(this._thing);
            set
            {
                if (this._field != null)
                    this._field.SetValue(this._thing, value);
                else
                    this._property.SetValue(this._thing, value, null);
            }
        }

        public float min => this._min;

        public float max => this._max;

        public float inc => this._inc;

        public FieldBinding(object thing, string field, float min = 0f, float max = 1f, float increment = 0.1f)
        {
            this._thing = thing;
            this._field = thing.GetType().GetField(field);
            if (this._field == null)
                this._property = thing.GetType().GetProperty(field);
            this._min = min;
            this._max = max;
            this._inc = increment;
            this.Construct();
        }

        public FieldBinding(System.Type thing, string field, float min = 0f, float max = 1f, float increment = 0.1f)
        {
            this._thing = thing;
            this._field = thing.GetField(field);
            if (this._field == null)
                this._property = thing.GetProperty(field);
            this._min = min;
            this._max = max;
            this._inc = increment;
            this.Construct();
        }

        private void Construct()
        {
            if (this.value == null || !this.value.GetType().IsEnum)
                return;
            this._min = 0f;
            this._max = Enum.GetValues(this.value.GetType()).Length - 1;
        }
    }
}
