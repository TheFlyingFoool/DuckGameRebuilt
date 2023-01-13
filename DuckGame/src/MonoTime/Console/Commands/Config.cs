using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Get or Modify config fields from the console")]
        public static object Config(string fieldId, string serializedValue = null)
        {
            object getValue(MemberInfo mi)
            {
                return mi switch
                {
                    FieldInfo fi => fi.GetValue(null),
                    PropertyInfo pi => pi.GetMethod?.Invoke(null, null),
                    _ => throw new Exception("Unsupported AutoConfig field type")
                };
            }

            switch (fieldId.ToUpper())
            {
                case "%SAVE":
                    AutoConfigHandler.SaveAll(false);
                    return null;
                case "%LOAD":
                    AutoConfigHandler.LoadAll();
                    return null;
                case "%LIST":
                    return AutoConfigFieldAttribute.All
                        .Select(x => $"{x.MemberInfo.Name}: |DGBLUE|{getValue(x.MemberInfo)}|PREV|");
            }

            List<AutoConfigFieldAttribute> all = AutoConfigFieldAttribute.All;

            if (!all.TryFirst(x => (x.ShortName ?? x.Id ?? x.MemberInfo.Name).CaselessEquals(fieldId), out AutoConfigFieldAttribute attribute))
                throw new Exception($"No configuration field found with ID: {fieldId}");

            object oldVal = getValue(attribute.MemberInfo);

            if (serializedValue is null)
                return oldVal;

            object newVal = FireSerializer.Deserialize(attribute.MemberInfo switch
            {
                FieldInfo fi => fi.FieldType,
                PropertyInfo pi => pi.PropertyType,
                _ => throw new Exception("Unsupported AutoConfig field type")
            }, serializedValue);

            switch (attribute.MemberInfo)
            {
                case FieldInfo fieldInfo:
                {
                    fieldInfo.SetValue(null, newVal);
                    break;
                }
                case PropertyInfo propertyInfo:
                {
                    propertyInfo.SetMethod?.Invoke(null, new[] { newVal });
                    break;
                }
                default:
                    throw new Exception("Unsupported AutoConfig field type");
            }
            
            return $"|DGBLUE|Modified value of field [|PINK|{attribute.MemberInfo.Name}|DGBLUE|] from [|GREEN|{oldVal}|DGBLUE|] to [|GREEN|{newVal}|DGBLUE|]";
        }
    }
}