using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class Int64Interpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(long);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return long.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to long: {fromString}");
            }

            public IList<string> Options(string fromString, Type specificType, CommandRunner engine)
            {
                return Array.Empty<string>();
            }
        }
    }
}