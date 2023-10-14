using System;
using AddedContent.Firebreak.DebuggerTools.Manager.Interface.Console;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        public class DSHVariableInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(DSHVariable);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return Commands.VariableRegister.TryGetValue(fromString, out DSHVariable value)
                    ? value
                    : new Exception($"Variable doesn't exist: {fromString}");
            }
        }
    }
}