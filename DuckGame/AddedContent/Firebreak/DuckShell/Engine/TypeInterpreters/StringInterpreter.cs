using AddedContent.Firebreak;
using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class StringInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(string);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return fromString;
            }

            public string[] Options(string fromString, Type specificType, CommandRunner engine)
            {
                return Array.Empty<string>();
            }
        }
    }
}