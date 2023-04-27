using AddedContent.Firebreak.DebuggerTools.Manager.Interface.Console;
using System;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public class MMVariableInterpreter : ITypeInterpreter
    {
        public Type ParsingType { get; } = typeof(MMVariable);
        public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
        {
            if (!fromString.StartsWith("dg/"))
            {
                return Commands.VariableRegister.TryGetValue(fromString, out MMVariable value)
                    ? value
                    : new Exception($"Variable doesn't exist: {fromString}");
            }
            else
            {
                fromString = fromString.Remove(0, 3);
                return new NotImplementedException("dg thing search not implemented yet :D");
            }
        }
    }
}