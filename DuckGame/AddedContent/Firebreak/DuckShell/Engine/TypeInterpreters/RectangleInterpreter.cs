using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class RectangleInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(Rectangle);

            public ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return Rectangle.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to Rectangle: {fromString}");
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return Array.Empty<string>();
            }
        }
    }
}