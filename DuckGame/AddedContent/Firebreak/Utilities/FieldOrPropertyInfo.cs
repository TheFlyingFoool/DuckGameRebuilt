using System;
using System.Reflection;

namespace DuckGame
{
    public class FieldOrPropertyInfo : MemberInfo
    {
        private readonly MemberInfo _fieldOrProperty;

        public FieldOrPropertyInfo(MemberInfo fieldOrProperty)
        {
            if (fieldOrProperty.MemberType is not MemberTypes.Property and not MemberTypes.Field)
                throw new Exception("FieldOrProperty member has to be a field or property (duh)");
            
            _fieldOrProperty = fieldOrProperty;
        }
        
        public Type FieldOrPropertyType => _fieldOrProperty switch
        {
            FieldInfo fieldInfo => fieldInfo.FieldType,
            PropertyInfo propertyInfo => propertyInfo.PropertyType,
            _ => throw new InvalidOperationException()
        };

        public object GetValue(object obj)
        {
            return _fieldOrProperty switch
            {
                FieldInfo fieldInfo => fieldInfo.GetValue(obj),
                PropertyInfo propertyInfo => propertyInfo.GetMethod?.Invoke(obj, null),
                _ => throw new InvalidOperationException()
            };
        }

        public void SetValue(object obj, object value)
        {
            switch (_fieldOrProperty)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(obj, value);
                    break;

                case PropertyInfo propertyInfo:
                    propertyInfo.SetMethod?.Invoke(obj, new[] {value});
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
        
        public override object[] GetCustomAttributes(bool inherit) => _fieldOrProperty.GetCustomAttributes(inherit);
        public override bool IsDefined(Type attributeType, bool inherit) => _fieldOrProperty.IsDefined(attributeType, inherit);
        public override MemberTypes MemberType => _fieldOrProperty.MemberType;
        public override string Name => _fieldOrProperty.Name;
        public override Type DeclaringType => _fieldOrProperty.DeclaringType;
        public override Type ReflectedType => _fieldOrProperty.ReflectedType;
        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => _fieldOrProperty.GetCustomAttributes(attributeType, inherit);
    }
}