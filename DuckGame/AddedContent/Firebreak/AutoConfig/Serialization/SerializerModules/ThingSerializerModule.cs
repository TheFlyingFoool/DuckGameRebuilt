using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AddedContent.Firebreak;

namespace DuckGame
{
    [Marker.FireSerializer]
    public class ThingSerializerModule : IFireSerializerModule<Thing>
    {
        public string Serialize(Thing obj)
        {
            Type thingType = obj.GetType();
            StringBuilder result = new();

            result.Append(thingType.Name);
            
            if (!Editor.HasConstructorParameter(thingType))
                throw new Exception($"Thing of type {thingType} cannot be serialized");
                
            Thing defaultThing = Editor.CreateThing(thingType);

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase;
            IEnumerable<FieldOrPropertyInfo> serializableMembers = thingType
                .GetMembers(flags)
                .Where(x => x.MemberType is MemberTypes.Field or MemberTypes.Property)
                .Select(x => new FieldOrPropertyInfo(x))
                .Where(x => FireSerializer.IsSerializable(x.FieldOrPropertyType));

            foreach (FieldOrPropertyInfo fieldInfo in serializableMembers)
            {
                object wantedValue = fieldInfo.GetValue(obj);
                object defaultValue = fieldInfo.GetValue(defaultThing);
                
                if (wantedValue == defaultValue)
                    continue;
                
                result.Append(':');
                result.Append(fieldInfo.Name);
                result.Append('=');
                result.Append(FireSerializer.Serialize(wantedValue));
            }

            return result.ToString();
        }

        public Thing Deserialize(string s)
        {
            foreach (Type thingType in Editor.ThingTypes
                         .Where(thingType => thingType.Name.ToLowerInvariant() == Regex.Replace(s, ":.*", "")))
            {
                if (!Editor.HasConstructorParameter(thingType))
                    throw new Exception($"{thingType.Name} can not be spawned this way.");
                
                Thing thing = Editor.CreateThing(thingType);

                IEnumerable<string[]> properties = s.Split(':').Skip(1).Select(x => x.Split('='));

                foreach (string[] nameValuePair in properties)
                {
                    string name = nameValuePair[0];
                    string valueSerialized = nameValuePair[1];
                    
                    const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase;
                    FieldOrPropertyInfo field = new(thingType
                            .GetMembers(flags)
                            .FirstOrDefault(x => x.MemberType is MemberTypes.Field or MemberTypes.Property
                                && x.Name == name)
                        ?? throw new Exception($"Member not found: {thingType}.{name}"));

                    object value = FireSerializer.Deserialize(field.FieldOrPropertyType, valueSerialized);
                
                    field.SetValue(thing, value);
                }
                
                return thing;
            }

            throw new Exception($"Thing of type ({s}) was not found.");
        }

        public bool CanSerialize(Type t)
        {
            return t.InheritsFrom(typeof(Thing));
        }

        string IFireSerializerModule.Serialize(object obj) => Serialize((Thing)obj);
        object IFireSerializerModule.Deserialize(string s) => Deserialize(s);
    }
}