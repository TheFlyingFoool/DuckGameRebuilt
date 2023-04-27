using AddedContent.Firebreak.DebuggerTools.Manager.Interface.Console;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        public static Dictionary<string, MMVariable> VariableRegister = new();

        [MMCommand(Description = "Creates a variable with the provided value.")]
        public static void Set(string name, string value)
        {
            if (VariableRegister.TryGetValue(name, out MMVariable variable))
                variable.Value = value;
            else VariableRegister.Add(name, new MMVariable()
            {
                Name = name,
                Value = value
            });
        }
        
        [MMCommand(Description = "Drops a variable, and returns its value.")]
        public static string Pop(MMVariable variable)
        {
            VariableRegister.Remove(variable.Name);
            return variable.Value;
        }
        
        [MMCommand(Description = "Retrieves the value of a variable by its name.")]
        public static object Get(MMVariable variable)
        {
            return variable.Value ?? "<NULL>";
        }
        
        [MMCommand(Description = "Lists all the currently registered variables")]
        public static string VarList()
        {
            return JsonConvert.SerializeObject(VariableRegister, Formatting.Indented);
        }
    }
}