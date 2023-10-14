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

        [Marker.DSHCommand(Description = "Creates a variable with the provided value.")]
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
        
        [Marker.DSHCommand(Description = "Drops a variable, and returns its value.")]
        public static string Pop(DSHVariable variable)
        {
            VariableRegister.Remove(variable.Name);
            return variable.Value;
        }
        
        [Marker.DSHCommand(Description = "Retrieves the value of a variable by its name.")]
        public static object Get(DSHVariable variable)
        {
            return variable.Value;
        }
        
        [Marker.DSHCommand(Description = "Lists all the currently registered variables")]
        public static string VarList()
        {
            return JsonConvert.SerializeObject(VariableRegister.ToDictionary(x => x.Key, x => x.Value.Value), Formatting.Indented);
        }
    }
}