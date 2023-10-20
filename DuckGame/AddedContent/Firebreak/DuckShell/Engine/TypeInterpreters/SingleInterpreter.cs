using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class SingleInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(float);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return float.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to float: {fromString}");
            }

            public IList<string> Options(string fromString, Type specificType, CommandRunner engine)
            {
                return Array.Empty<string>();
            }
        }
    }
}