using DuckGame;
using DuckGame.ConsoleEngine;
using DuckGame.ConsoleEngine.TypeInterpreters;
using DuckGame.ConsoleInterface;
using DuckShell.Manager.Interface.Console;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AddedContent.Firebreak.DuckShell.Implementation
{
    public class DevConsoleDSHWrapper : IDSHConsole
    {
        public CommandRunner Shell { get; set; }
        public bool Active { get; set; }
        public List<DSHConsoleLine> Lines;

        public DevConsoleDSHWrapper()
        {
            Shell = new CommandRunner();
            Active = false;
            Lines = new List<DSHConsoleLine>();
            
            // TODO: rewrite to use marker system
            Shell.AddCommandsUsingAttribute(typeof(Commands).GetMethods());
            Shell.AddTypeInterpretters(typeof(TypeInterpreters).GetNestedTypes().Select(t => t.GetTypeInfo()));
        }

        [Marker.PostInitialize]
        public static void StaticInitialize()
        {
            Commands.console = new DevConsoleDSHWrapper();
        }
        
        public void Clear()
        {
            DevConsoleCommands.Clear();
        }
        
        public void WriteLine(object o, DSHConsoleLine.Significance significance)
        {
            Lines.Add(new DSHConsoleLine(o.ToString(), significance));
            
            // debug, remove later
            DevConsole.LogComplexMessage($"{significance.ToString().ToUpper()} {o}", Color.White);
        }

        public void Run(string command, bool byUser)
        {
            if (byUser)
            {
                WriteLine(command, DSHConsoleLine.Significance.User);
            }

            ValueOrException<object> valueOrException = Shell.Run(command);
            
            if (valueOrException.Error is { } error)
            {
                WriteLine(error.ToString(), DSHConsoleLine.Significance.Error);
            }
            else if (valueOrException.Value is not null)
            {
                WriteLine(valueOrException.Value, DSHConsoleLine.Significance.Response);
            }
        }
    }
}