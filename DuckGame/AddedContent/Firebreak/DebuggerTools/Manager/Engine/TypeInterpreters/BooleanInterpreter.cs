using DuckGame.ConsoleEngine;
using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public class BooleanInterpreter : ITypeInterpreter
    {
        public Type ParsingType { get; } = typeof(bool);
        public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
        {
            return bool.TryParse(fromString, out var val)
                ? val 
                : new Exception($"Unable to parse to bool: {fromString}");
        }
    }
}