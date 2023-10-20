using DuckGame;
using DuckGame.ConsoleEngine;
using DuckGame.ConsoleEngine.TypeInterpreters;
using DuckGame.ConsoleInterface;
using DuckShell.Manager.Interface.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AddedContent.Firebreak.DuckShell.Implementation
{
    public class DevConsoleDSHWrapper : IDSHConsole
    {
        public CommandRunner Shell { get; set; }
        public bool Active { get; set; }
        
        internal static readonly List<Marker.DevConsoleCommandAttribute> AttributeCommands = new();
        internal static readonly List<TypeInfo> TypeInterpreterInfos = new();

        public DevConsoleDSHWrapper()
        {
            Shell = new CommandRunner();
            Active = false;

            foreach (Marker.DevConsoleCommandAttribute command in AttributeCommands)
            {
                Shell.AddCommand(command);
            }

            foreach (TypeInfo typeInfo in TypeInterpreterInfos)
            {
                ITypeInterpreter interpreterInstance = (ITypeInterpreter) Activator.CreateInstance(typeInfo)!;
            
                Shell.TypeInterpreterModules.Add(interpreterInstance);
                Shell.TypeInterpreterModulesMap.Add(interpreterInstance.ParsingType, interpreterInstance);
            }
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
            // Color significanceColor = significance switch
            // {
            //     DSHConsoleLine.Significance.Neutral => Color.White,
            //     DSHConsoleLine.Significance.User => Color.MediumPurple,
            //     DSHConsoleLine.Significance.Response => Color.Aquamarine,
            //     DSHConsoleLine.Significance.Highlight => Color.Yellow,
            //     DSHConsoleLine.Significance.Error => Color.Red,
            //     DSHConsoleLine.Significance.VeryFuckingImportant => Color.Purple,
            //     _ => throw new ArgumentOutOfRangeException(nameof(significance), significance, null)
            // };
            //
            // DevConsole.Log($"{significance.ToString().ToUpper().SetLengthLogically(4)} {o}", significanceColor);
            DevConsole.Log(significance switch
            {
                DSHConsoleLine.Significance.User => $"~@u {o}",
                _ => $"{o}",
            }, significance switch
            {
                DSHConsoleLine.Significance.User => new Color("#afeb8f"),
                DSHConsoleLine.Significance.Response => Color.Aquamarine,
                DSHConsoleLine.Significance.Error => Colors.DGRed,
                _ => Color.White,
            });
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