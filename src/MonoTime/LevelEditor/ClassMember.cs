// Decompiled with JetBrains decompiler
// Type: DuckGame.ClassMember
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public FieldInfo field => this._fieldInfo;

        public PropertyInfo property => this._propertyInfo;

        public System.Type declaringType => this._declaringType;

        public bool isPrivate => this._isPrivate;

        public string name => this._name;

        public System.Type type => this._fieldInfo != null ? this._fieldInfo.FieldType : this._propertyInfo.PropertyType;

        public bool isConst => this._fieldInfo != null && this._fieldInfo.IsLiteral && !this._fieldInfo.IsInitOnly;

        public bool hasGetterAndSetter
        {
            get
            {
                if (this._accessor == null)
                    this._accessor = Editor.GetAccessorInfo(this._declaringType, this._name, this._fieldInfo, this._propertyInfo);
                return this._accessor.getAccessor != null && this._accessor.setAccessor != null;
            }
        }

        public object GetValue(object instance)
        {
            if (this._accessor == null)
                this._accessor = Editor.GetAccessorInfo(this._declaringType, this._name, this._fieldInfo, this._propertyInfo);
            return this._accessor.getAccessor(instance);
        }

        public void SetValue(object instance, object value)
        {
            if (this._accessor == null)
            {
                if (this._fieldInfo != null)
                {
                    string name = this._fieldInfo.Name;
                    System.Type fieldType = this._fieldInfo.FieldType;
                }
                if (this._propertyInfo != null)
                {
                    string name = this._propertyInfo.Name;
                    System.Type propertyType = this._propertyInfo.PropertyType;
                }
                this._accessor = Editor.GetAccessorInfo(this._declaringType, this._name, this._fieldInfo, this._propertyInfo);
            }
            this._accessor.setAccessor(instance, value);
        }

        public ClassMember(string n, System.Type declaringTp, FieldInfo field)
        {
            this._fieldInfo = field;
            this._name = n;
            this._declaringType = declaringTp;
            this._isPrivate = field.IsPrivate;
        }

        public ClassMember(string n, System.Type declaringTp, PropertyInfo property)
        {
            this._propertyInfo = property;
            this._name = n;
            this._declaringType = declaringTp;
        }
    }
}
