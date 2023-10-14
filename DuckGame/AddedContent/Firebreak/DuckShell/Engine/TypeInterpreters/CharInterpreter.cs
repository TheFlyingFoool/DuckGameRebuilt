using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        public class CharInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(char);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return char.TryParse(fromString, out var val)
                    ? val
                    : new Exception($"Unable to parse to char: {fromString}");
            }
        }
    }
}