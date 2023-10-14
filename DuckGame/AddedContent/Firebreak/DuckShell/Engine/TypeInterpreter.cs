using System;

namespace DuckGame.ConsoleEngine
{
    public interface ITypeInterpreter
    {
        /// <summary>
        /// The type the interpreter is designed to parse for.
        /// </summary>
        Type ParsingType { get; }
        ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine);
    }
}