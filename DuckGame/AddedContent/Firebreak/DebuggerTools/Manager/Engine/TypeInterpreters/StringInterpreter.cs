using DuckGame.ConsoleEngine;
using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public class StringInterpreter : ITypeInterpreter
    {
        public Type ParsingType { get; } = typeof(string);

        public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
        {
            return fromString;
        }
    }
}