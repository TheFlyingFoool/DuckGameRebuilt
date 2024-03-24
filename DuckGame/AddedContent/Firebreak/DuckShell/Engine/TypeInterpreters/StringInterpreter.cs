using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class StringInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(string);

            public ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return fromString;
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return Array.Empty<string>();
            }
        }
    }
}