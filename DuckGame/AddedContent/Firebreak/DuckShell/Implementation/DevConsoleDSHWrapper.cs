using DuckGame;
using DuckGame.ConsoleEngine;
using DuckGame.ConsoleEngine.TypeInterpreters;
using DuckGame.ConsoleInterface;
using DuckShell.Manager.Interface.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AddedContent.Firebreak.DuckShell.Implementation
{
    public class DevConsoleDSHWrapper : IDSHConsole
    {
        public CommandRunner Shell { get; set; }
        public bool Active { get; set; }
        
        public static readonly List<MethodInfo> AttributeCommandInfos = new();
        public static readonly List<TypeInfo> TypeInterpreterInfos = new();

        public DevConsoleDSHWrapper()
        {
            Shell = new CommandRunner();
            Active = false;
            
            Shell.AddCommandsUsingAttribute(AttributeCommandInfos);
            Shell.AddTypeInterpretters(TypeInterpreterInfos);
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
            Color significanceColor = significance switch
            {
                DSHConsoleLine.Significance.Neutral => Color.White,
                DSHConsoleLine.Significance.User => Color.MediumPurple,
                DSHConsoleLine.Significance.Response => Color.Aquamarine,
                DSHConsoleLine.Significance.Highlight => Color.Yellow,
                DSHConsoleLine.Significance.Error => Color.Red,
                DSHConsoleLine.Significance.VeryFuckingImportant => Color.Purple,
                _ => throw new ArgumentOutOfRangeException(nameof(significance), significance, null)
            };
            
            DevConsole.Log($"{significance.ToString().ToUpper(),-9} {o}", significanceColor);
        }

        public void Run(string command, bool byUser)
        {
            if (byUser)
            {
                WriteLine(command, DSHConsoleLine.Significance.User);
                DevConsole.FlushPendingLines();
            }

            ValueOrException<object> valueOrException = Shell.Run(command);
            
            if (valueOrException.Error is { } error)
            {
                WriteLine($"{(error.GetType() == typeof(Exception) ? "" : $"{error.GetType()}: ")}{error.Message}", DSHConsoleLine.Significance.Error);
            }
            else if (valueOrException.Value is not null)
            {
                WriteLine(valueOrException.Value, DSHConsoleLine.Significance.Response);
            }
        }
    }
}