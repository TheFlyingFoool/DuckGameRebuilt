using AddedContent.Firebreak;
using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class UInt32Interpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(uint);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return uint.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to uint: {fromString}");
            }

            public string[] Options(string fromString, Type specificType, CommandRunner engine)
            {
                return Array.Empty<string>();
            }
        }
    }
}