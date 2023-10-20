using AddedContent.Firebreak;
using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class CharInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(char);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return char.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to char: {fromString}");
            }

            public string[] Options(string fromString, Type specificType, CommandRunner engine)
            {
                return Array.Empty<string>();
            }
        }
    }
}