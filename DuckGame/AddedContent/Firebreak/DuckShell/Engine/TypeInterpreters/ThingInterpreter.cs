using AddedContent.Firebreak;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class ThingInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(Thing);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                fromString = fromString.ToLowerInvariant();

                if (fromString is "%r" or "gun")
                    return ItemBoxRandom.GetRandomItem();

                if (specificType == typeof(TeamHat))
                {
                    Team t = Teams.all.FirstOrDefault(x => x.name.ToLower() == fromString);
                    return t != null ? new TeamHat(0f, 0f, t) : new Exception(
                        $"Argument ({fromString}) should be the name of a team");
                }

                foreach (Type thingType in Editor.ThingTypes
                             .Where(thingType => thingType.Name.ToLowerInvariant() == fromString))
                {
                    if (!Editor.HasConstructorParameter(thingType))
                        return new Exception($"{thingType.Name} can not be spawned this way.");
                    return !specificType.IsAssignableFrom(thingType) ? new Exception(
                        $"Wrong object type (requires {specificType.Name}).") : Editor.CreateThing(thingType);
                }

                return new Exception($"{specificType.Name} of type ({fromString}) was not found.");
            }

            public IList<string> Options(string fromString, Type specificType, CommandRunner engine)
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