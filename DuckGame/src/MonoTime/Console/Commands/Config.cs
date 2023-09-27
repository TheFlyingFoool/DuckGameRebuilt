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
                        .Select(x => $"{x.MemberInfo.Name}: |DGBLUE|{FireSerializer.Serialize(x.Value)}|PREV|");
            }

            List<AutoConfigFieldAttribute> all = AutoConfigFieldAttribute.All;

            if (!all.TryFirst(x => (x.Id ?? x.MemberInfo.Name).CaselessEquals(fieldId), out AutoConfigFieldAttribute attribute))
                throw new Exception($"No configuration field found with ID: {fieldId}");

            object oldVal = attribute.Value;
            string oldReserializedValue = FireSerializer.Serialize(oldVal);

            if (serializedValue is null)
                return oldReserializedValue;

            object newVal = FireSerializer.Deserialize(attribute.MemberType, serializedValue);
            string newReserializedValue = FireSerializer.Serialize(newVal);

            attribute.Value = newVal;
            
            return $"|PINK|{attribute.MemberInfo.Name}|WHITE|: |WHITE|[|GREEN|{oldReserializedValue}|WHITE|] to [|GREEN|{newReserializedValue}|WHITE|]";
        }
    }
}