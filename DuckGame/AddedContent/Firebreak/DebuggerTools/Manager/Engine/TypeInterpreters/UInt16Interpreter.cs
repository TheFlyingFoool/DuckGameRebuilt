using DuckGame.ConsoleEngine;
using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public class UInt16Interpreter : ITypeInterpreter
    {
        public Type ParsingType { get; } = typeof(ushort);
        public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
        {
            return ushort.TryParse(fromString, out var val)
                ? val 
                : new Exception($"Unable to parse to ushort: {fromString}"); 
        }
    }
}