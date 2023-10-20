using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class DoubleInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(double);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return double.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to double: {fromString}");
            }

            public IList<string> Options(string fromString, Type specificType, CommandRunner engine)
            {
                return Array.Empty<string>();
            }
        }
    }
}