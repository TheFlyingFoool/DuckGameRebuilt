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
        public class DuckInterpreter : ITypeInterpreter
        {
            public virtual Type ParsingType { get; } = typeof(Duck);

            public virtual ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                Profile profile = Extensions.GetProfSafe(fromString);

                if (profile is null)
                    return new Exception($"No profile matching: {fromString}");

                if (profile.duck is null || profile.duck.dead)
                    return new Exception($"{fromString}'s duck does not exist");
                
                return profile.duck;
            }

            public IList<string> Options(string fromString, Type specificType, CommandRunner engine)
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