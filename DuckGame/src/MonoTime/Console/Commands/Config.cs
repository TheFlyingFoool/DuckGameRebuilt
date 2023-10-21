using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Get or Modify config fields from the console")]
        [return: PrettyPrint]
        public static object Config([ConfigAutoCompl] string fieldId, string serializedValue = null)
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
                    return Marker.AutoConfigAttribute.All
                        .Select(x => $"{x.Member.Name}: |DGBLUE|{FireSerializer.Serialize(x.Value)}|PREV|");
            }

            List<Marker.AutoConfigAttribute> all = Marker.AutoConfigAttribute.All;

            if (!all.TryFirst(x => (x.Id ?? x.Member.Name).CaselessEquals(fieldId), out Marker.AutoConfigAttribute attribute))
                throw new Exception($"No configuration field found with ID: {fieldId}");

            object oldVal = attribute.Value;
            string oldReserializedValue = FireSerializer.Serialize(oldVal);

            if (serializedValue is null)
                return oldReserializedValue;

            object newVal = FireSerializer.Deserialize(attribute.MemberType, serializedValue);
            string newReserializedValue = FireSerializer.Serialize(newVal);

            attribute.Value = newVal;
            
            return $"|PINK|{attribute.Member.Name}|WHITE|: |WHITE|[|GREEN|{oldReserializedValue}|WHITE|] to [|GREEN|{newReserializedValue}|WHITE|]";
        }
    }
}