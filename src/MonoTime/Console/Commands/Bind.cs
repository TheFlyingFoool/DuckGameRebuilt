using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    public static List<ConsoleBind> Binds = new();

    public record ConsoleBind(string hotkey, string command)
    {
        public string hotkey { get; set; } = hotkey;
        public string command { get; set; } = command;
    }

    // [DevConsoleCommand(Aliases = new[] { "binds" })]
    public static void Bind(BindAction action)
    {
        switch (action)
        {
            case BindAction.Add:
                {
                    
                    break;
                }
            case BindAction.Remove:
                {
                    
                    break;
                }
            case BindAction.List:
                {
                    
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }

    public enum BindAction
    {
        Add,
        Remove,
        List
    }
}