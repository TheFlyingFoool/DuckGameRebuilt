using AddedContent.Firebreak;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class ProfileInterpreter : ITypeInterpreter
        {
            public virtual Type ParsingType { get; } = typeof(Profile);

            public virtual ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                Profile profile = Extensions.GetProfSafe(fromString);
                
                return profile is null
                    ? ValueOrException<object>.FromError($"No profile matching: {fromString}") 
                    : ValueOrException<object>.FromValue(profile);
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                List<string> options = new() {
                    "p1",
                    "p2",
                    "p3",
                    "p4",
                    "p5",
                    "p6",
                    "p7",
                    "p8",
                    "o1",
                    "o2",
                    "o3",
                    "o4",
                    "player1",
                    "player2",
                    "player3",
                    "player4",
                    "player5",
                    "player6",
                    "player7",
                    "player8",
                    "observer1",
                    "observer2",
                    "observer3",
                    "observer4",
                    "white",
                    "gray",
                    "grey",
                    "yellow",
                    "orange",
                    "brown",
                    "green",
                    "pink",
                    "rose",
                    "blue",
                    "purple",
                    "me",
                };
                
                options.AddRange(Profiles.active.Select(x => x.name.CleanFormatting()));

                return options;
            }
        }
    }
}