using AddedContent.Firebreak;
using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class Int16Interpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(short);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return short.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to short: {fromString}");
            }
        }
    }
}