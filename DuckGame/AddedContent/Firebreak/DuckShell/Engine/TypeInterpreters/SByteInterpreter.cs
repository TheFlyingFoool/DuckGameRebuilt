using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        public class SByteInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(sbyte);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return sbyte.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to sbyte: {fromString}");
            }
        }
    }
}