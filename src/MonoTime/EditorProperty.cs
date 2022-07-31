// Decompiled with JetBrains decompiler
// Type: DuckGame.EditorProperty`1
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class EditorProperty<T>
    {
        private T _value;
        public string name;
        public string _tooltip;
        private float _min;
        private float _max = 1f;
        private float _increment = 0.1f;
        private string _minSpecial;
        private bool _isTime;
        private bool _isLevel;
        private Thing _notify;
        private string _section = "";

        public T value
        {
            get => this._value;
            set
            {
                this._value = value;
                if (this._notify == null)
                    return;
                this._notify.EditorPropertyChanged(this);
            }
        }

        public EditorPropertyInfo info => new EditorPropertyInfo()
        {
            value = _value,
            min = this._min,
            max = this._max,
            increment = this._increment,
            minSpecial = this._minSpecial,
            isTime = this._isTime,
            isLevel = this._isLevel,
            tooltip = this._tooltip,
            name = this.name
        };

        public string section => this._section;

        public EditorProperty(
          T val,
          Thing notify = null,
          float min = 0f,
          float max = 1f,
          float increment = 0.1f,
          string minSpecial = null,
          bool isTime = false,
          bool isLevel = false)
        {
            this._value = val;
            this._min = min;
            this._max = max;
            this._increment = increment;
            this._minSpecial = minSpecial;
            this._isTime = isTime;
            this._notify = notify;
            this._isLevel = isLevel;
        }

        public EditorProperty(
          T val,
          string varSection,
          Thing notify = null,
          float min = 0f,
          float max = 1f,
          float increment = 0.1f,
          string minSpecial = null,
          bool isTime = false,
          bool isLevel = false)
        {
            this._value = val;
            this._min = min;
            this._max = max;
            this._increment = increment;
            this._minSpecial = minSpecial;
            this._isTime = isTime;
            this._notify = notify;
            this._section = varSection;
            this._isLevel = isLevel;
        }

        /// <summary>
        /// This is left here for legacy mod support, DON'T USE IT IT BREAKS PROPERTIES!
        /// </summary>
        /// <param name="val"></param>
        public static implicit operator EditorProperty<T>(T val) => new EditorProperty<T>(val);

        public static implicit operator T(EditorProperty<T> val) => val._value;
    }
}
