using DuckGame.ConsoleEngine;
using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public class ByteInterpreter : ITypeInterpreter
    {
        public Type ParsingType { get; } = typeof(byte);
        public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
        {
            return byte.TryParse(fromString, out var val)
                ? val 
                : new Exception($"Unable to parse to byte: {fromString}"); 
        }
    }
}