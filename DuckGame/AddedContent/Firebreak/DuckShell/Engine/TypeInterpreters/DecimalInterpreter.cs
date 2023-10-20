using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class DecimalInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(decimal);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return decimal.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to decimal: {fromString}");
            }

            public IList<string> Options(string fromString, Type specificType, CommandRunner engine)
            {
                return Array.Empty<string>();
            }
        }
    }
}