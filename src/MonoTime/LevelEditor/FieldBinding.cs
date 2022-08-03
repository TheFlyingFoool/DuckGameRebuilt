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

        public object thing => _thing;

        public virtual object value
        {
            get => !(_field != null) ? _property.GetValue(_thing, null) : _field.GetValue(_thing);
            set
            {
                if (_field != null)
                    _field.SetValue(_thing, value);
                else
                    _property.SetValue(_thing, value, null);
            }
        }

        public float min => _min;

        public float max => _max;

        public float inc => _inc;

        public FieldBinding(object thing, string field, float min = 0f, float max = 1f, float increment = 0.1f)
        {
            _thing = thing;
            _field = thing.GetType().GetField(field);
            if (_field == null)
                _property = thing.GetType().GetProperty(field);
            _min = min;
            _max = max;
            _inc = increment;
            Construct();
        }

        public FieldBinding(System.Type thing, string field, float min = 0f, float max = 1f, float increment = 0.1f)
        {
            _thing = thing;
            _field = thing.GetField(field);
            if (_field == null)
                _property = thing.GetProperty(field);
            _min = min;
            _max = max;
            _inc = increment;
            Construct();
        }

        private void Construct()
        {
            if (value == null || !value.GetType().IsEnum)
                return;
            _min = 0f;
            _max = Enum.GetValues(value.GetType()).Length - 1;
        }
    }
}
