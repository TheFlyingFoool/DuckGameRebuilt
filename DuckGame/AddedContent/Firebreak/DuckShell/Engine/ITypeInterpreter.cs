using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public interface ITypeInterpreter
    {
        /// <summary>
        /// The type the interpreter is designed to parse for.
        /// </summary>
        Type ParsingType { get; }
        ValueOrException<object> ParseString(string fromString, Type specificType, TypeInterpreterParseContext context);
        IList<string> Options(string fromString, Type specificType, TypeInterpreterParseContext context);
    }
}