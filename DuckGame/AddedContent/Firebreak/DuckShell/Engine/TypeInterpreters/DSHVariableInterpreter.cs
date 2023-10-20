using AddedContent.Firebreak;
using System;
using AddedContent.Firebreak.DebuggerTools.Manager.Interface.Console;

namespace DuckGame.ConsoleEngine.TypeInterpreters
{
    public static partial class TypeInterpreters
    {
        [Marker.DSHTypeInterpreterAttribute]
        public class DSHVariableInterpreter : ITypeInterpreter
        {
            public Type ParsingType { get; } = typeof(DSHVariable);

            public ValueOrException<object> ParseString(string fromString, Type specificType, CommandRunner engine)
            {
                return Commands.VariableRegister.TryGetValue(fromString, out DSHVariable value)
                    ? value
                    : new Exception($"Variable doesn't exist: {fromString}");
            }

            public string[] Options(string fromString, Type specificType, CommandRunner engine)
            {
                string[] options = new string[Commands.VariableRegister.Count];
                Commands.VariableRegister.Keys.CopyTo(options, 0);
                
                return options;
            }
        }
    }
}