using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class Int16Interpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(short);

            public ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return short.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to short: {fromString}");
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return Array.Empty<string>();
            }
        }
    }
}