using System;
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

            switch (fieldId)
            {
                case "%SAVE":
                    AutoConfigHandler.SaveAll(false);
                    return null;
                case "%LOAD":
                    AutoConfigHandler.LoadAll(AutoConfigHandler.LoadFrequency.All);
                    return null;
                case "%LIST":
                    return AutoConfigFieldAttribute.All
                        .Select(x => $"{x.MemberInfo.Name}: |DGBLUE|{getValue(x.MemberInfo)}|PREV|");
            }

            System.Collections.Generic.IReadOnlyList<MemberAttributePair<MemberInfo, AutoConfigFieldAttribute>> all = AutoConfigFieldAttribute.All;

            if (!all.TryFirst(x => (x.Attribute.ShortName ?? x.Attribute.Id ?? x.MemberInfo.Name).CaselessEquals(fieldId), out MemberAttributePair<MemberInfo, AutoConfigFieldAttribute> field))
                throw new Exception($"No configuration field found with ID: {fieldId}");

            object val = getValue(field.MemberInfo);

            if (serializedValue is null)
                return val;

            object newVal = FireSerializer.Deserialize(field.MemberInfo switch
            {
                FieldInfo fi => fi.FieldType,
                PropertyInfo pi => pi.PropertyType,
                _ => throw new Exception("Unsupported AutoConfig field type")
            }, serializedValue);

            switch (field.MemberInfo)
            {
                case FieldInfo fi:
                    {
                        fi.SetValue(null, val);
                        break;
                    }
                case PropertyInfo pi:
                    {
                        pi.SetMethod?.Invoke(null, new[] { val });
                        break;
                    }
                default:
                    throw new Exception("Unsupported AutoConfig field type");
            }
            return $"|DGBLUE|Modified value of field [|PINK|{field.MemberInfo.Name}|DGBLUE|] from [|GREEN|{val}|DGBLUE|] to [|GREEN|{newVal}|DGBLUE|]";
        }
    }
}