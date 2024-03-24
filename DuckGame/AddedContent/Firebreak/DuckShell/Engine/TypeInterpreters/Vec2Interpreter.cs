using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class Vec2Interpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(Vec2);

            public ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return Vec2.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to Vec2: {fromString}");
            }

            public IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context)
            {
                return Array.Empty<string>();
            }
        }
    }
}