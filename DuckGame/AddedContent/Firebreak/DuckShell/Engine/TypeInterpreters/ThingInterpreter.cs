using AddedContent.Firebreak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class ThingInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(Thing);

            public ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                IEnumerable<string[]> properties = fromString.Split(':').Skip(1).Select(x => x.Split('='));
                fromString = Regex.Replace(fromString.ToLowerInvariant(), ":.*", "");

                if (fromString is "%r" or "gun")
                    return ItemBoxRandom.GetRandomItem();

                if (specificType == typeof(TeamHat))
                {
                    Team t = Teams.all.FirstOrDefault(x => x.name.ToLower() == fromString);
                    return t != null ? new TeamHat(0f, 0f, t) : new Exception(
                        $"Argument ({fromString}) should be the name of a team");
                }

                Thing thing = null;

                foreach (Type thingType in Editor.ThingTypes
                             .Where(thingType => thingType.Name.ToLowerInvariant() == fromString))
                {
                    if (!Editor.HasConstructorParameter(thingType))
                        return new Exception($"{thingType.Name} can not be spawned this way.");
                    if (!specificType.IsAssignableFrom(thingType))
                    {
                        return new Exception($"Wrong object type (requires {specificType.Name}).");
                    }
                    else
                    {
                        thing = Editor.CreateThing(thingType);
                        break;
                    }
                }
                
                if (thing is null)
                    return new Exception($"{specificType.Name} of type ({fromString}) was not found.");

                Type type = thing.GetType();
                
                foreach (string[] nameValuePair in properties)
                {
                    string name = nameValuePair[0];
                    string valueSerialized = nameValuePair[1];
                    
                    const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase;
                    FieldOrPropertyInfo field = new(type
                            .GetMembers(flags)
                            .FirstOrDefault(x => x.MemberType is MemberTypes.Field or MemberTypes.Property
                                && x.Name == name)
                        ?? throw new Exception($"Member not found: {type}.{name}"));

                    object value = FireSerializer.Deserialize(field.FieldOrPropertyType, valueSerialized);
                
                    field.SetValue(thing, value);
                }

                return thing;
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                List<string> options = new() { "%r" };

                for (int i = 0; i < Editor.ThingTypes.Count; i++)
                {
                    Type type = Editor.ThingTypes[i];
                    
                    if (type.InheritsFrom(specificType))
                        options.Add(type.Name);
                }
                
                return options;
            }
        }
    }
}