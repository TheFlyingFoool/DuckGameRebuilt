using DuckGame.ConsoleEngine;
using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public class SingleInterpreter : ITypeInterpreter
    {
        public Type ParsingType { get; } = typeof(float);
        public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
        {
            return float.TryParse(fromString, out var val)
                ? val 
                : new Exception($"Unable to parse to float: {fromString}"); 
        }
    }
}