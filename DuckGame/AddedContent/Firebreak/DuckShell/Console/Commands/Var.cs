using AddedContent.Firebreak;
using AddedContent.Firebreak.DebuggerTools.Manager.Interface.Console;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        public static Dictionary<string, DSHVariable> VariableRegister = new();

        [Marker.DevConsoleCommand(Description = "Creates a variable with the provided value.", To = ImplementTo.DuckShell)]
        public static void Set(string name, string value)
        {
            if (VariableRegister.TryGetValue(name, out DSHVariable variable))
                variable.Value = value;
            else VariableRegister.Add(name, new DSHVariable()
            {
                Name = name,
                Value = value
            });
        }
        
        [Marker.DevConsoleCommand(Description = "Drops a variable, and returns its value.", To = ImplementTo.DuckShell)]
        public static string Pop(DSHVariable variable)
        {
            VariableRegister.Remove(variable.Name);
            return variable.Value;
        }
        
        [Marker.DevConsoleCommand(Description = "Retrieves the value of a variable by its name.", To = ImplementTo.DuckShell)]
        public static object Get(DSHVariable variable)
        {
            return variable.Value;
        }
        
        [return: PrettyPrint]
        [Marker.DevConsoleCommand(Description = "Lists all the currently registered variables", To = ImplementTo.DuckShell)]
        public static Dictionary<string, string> VarList()
        {
            return VariableRegister.ToDictionary(x => x.Key, x => x.Value.Value);
        }
    }
}