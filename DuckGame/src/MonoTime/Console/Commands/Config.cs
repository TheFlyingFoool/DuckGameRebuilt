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
                PropertyInfo pi = null;
                FieldInfo fi = mi as FieldInfo;
                if (fi == null)
                {
                    pi = mi as PropertyInfo;
                    if (pi == null)
                    {
                        throw new Exception("Unsupported AutoConfig field type");
                    }
                    return pi.GetMethod?.Invoke(null, null);
                }
                return fi.GetValue(null);
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
                        .Select(x => $"{x.field.Name}: |DGBLUE|{getValue(x.field)}|PREV|");
            }

            IReadOnlyList<AutoConfigFieldAttribute> all = AutoConfigFieldAttribute.All;

            if (!all.TryFirst(x => (x.ShortName ?? x.Id ?? x.field.Name).CaselessEquals(fieldId), out AutoConfigFieldAttribute AutoConfigField))
                throw new Exception($"No configuration field found with ID: {fieldId}");

            object val = getValue(AutoConfigField.field);

            if (serializedValue is null)
                return val;

            object newVal = FireSerializer.Deserialize((Type)AutoConfigField.field, serializedValue); ;
            PropertyInfo pi = null;
            FieldInfo fi = AutoConfigField.field as FieldInfo;
            if (fi == null)
            {
                pi = AutoConfigField.field as PropertyInfo;
                if (pi == null)
                {
                    throw new Exception("Unsupported AutoConfig field type");
                }
                pi.SetMethod?.Invoke(null, new[] { val });
            }
            else
            {

                fi.SetValue(null, val);
            }
            return $"|DGBLUE|Modified value of field [|PINK|{AutoConfigField.field.Name}|DGBLUE|] from [|GREEN|{val}|DGBLUE|] to [|GREEN|{newVal}|DGBLUE|]";
        }
    }
}