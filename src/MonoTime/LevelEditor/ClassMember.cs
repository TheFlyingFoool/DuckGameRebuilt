// Decompiled with JetBrains decompiler
// Type: DuckGame.ClassMember
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Reflection;

namespace DuckGame
{
    public class ClassMember
    {
        private FieldInfo _fieldInfo;
        private PropertyInfo _propertyInfo;
        private System.Type _declaringType;
        private bool _isPrivate;
        private string _name;
        private AccessorInfo _accessor;

        public FieldInfo field => _fieldInfo;

        public PropertyInfo property => _propertyInfo;

        public System.Type declaringType => _declaringType;

        public bool isPrivate => _isPrivate;

        public string name => _name;

        public System.Type type => _fieldInfo != null ? _fieldInfo.FieldType : _propertyInfo.PropertyType;

        public bool isConst => _fieldInfo != null && _fieldInfo.IsLiteral && !_fieldInfo.IsInitOnly;

        public bool hasGetterAndSetter
        {
            get
            {
                if (_accessor == null)
                    _accessor = Editor.GetAccessorInfo(_declaringType, _name, _fieldInfo, _propertyInfo);
                return _accessor.getAccessor != null && _accessor.setAccessor != null;
            }
        }

        public object GetValue(object instance)
        {
            if (_accessor == null)
                _accessor = Editor.GetAccessorInfo(_declaringType, _name, _fieldInfo, _propertyInfo);
            return _accessor.getAccessor(instance);
        }

        public void SetValue(object instance, object value)
        {
            if (_accessor == null)
            {
                if (_fieldInfo != null)
                {
                    string name = _fieldInfo.Name;
                    System.Type fieldType = _fieldInfo.FieldType;
                }
                if (_propertyInfo != null)
                {
                    string name = _propertyInfo.Name;
                    System.Type propertyType = _propertyInfo.PropertyType;
                }
                _accessor = Editor.GetAccessorInfo(_declaringType, _name, _fieldInfo, _propertyInfo);
            }
            _accessor.setAccessor(instance, value);
        }

        public ClassMember(string n, System.Type declaringTp, FieldInfo field)
        {
            _fieldInfo = field;
            _name = n;
            _declaringType = declaringTp;
            _isPrivate = field.IsPrivate;
        }

        public ClassMember(string n, System.Type declaringTp, PropertyInfo property)
        {
            _propertyInfo = property;
            _name = n;
            _declaringType = declaringTp;
        }
    }
}
