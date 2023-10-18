#nullable enable
using AddedContent.Firebreak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [Marker.AutoConfig]
        public static List<ConsoleBind> Binds = new();

        [Marker.DevConsoleCommand(
            Description = "Binds a command to a hotkey to be executed when pressed",
            Aliases = new[] { "binds" })]
        public static object Bind(BindAction action, string? hotkey = null, string? command = null)
        {
            switch (action)
            {
                case BindAction.Add:
                    {
                        if (hotkey is null)
                            throw new Exception("No hotkey provided");

                        if (command is null)
                            throw new Exception("No command provided");

                        if (!CustomKeyBinds.IsValidInput(hotkey))
                            return $"|DGRED|Key [{hotkey}] does not exist. Try running `bind keys` for a list of usable keys";
                        
                        Binds.Add(new ConsoleBind(hotkey, command));
                        return $"|DGBLUE|Added new binding at index [{Binds.Count - 1}] with hotkey [{hotkey}]";
                    }
                case BindAction.Remove:
                    {
                        if (hotkey is null)
                            throw new Exception("No index provided");

                        if (!int.TryParse(hotkey, out int index)
                            || index < 0
                            || index >= Binds.Count)
                            throw new Exception($"Cannot cast [{hotkey}] to a valid index");

                        Binds.RemoveAt(index);
                        return $"|DGBLUE|Removed binding at index [{index}]";
                    }
                case BindAction.List:
                    {
                        int numWidth = Binds.Count.ToString().Length;
                        return Binds.Select((x, i) => $"{i.ToString().PadLeft(numWidth, ' ')} | {x.hotkey}: {x.command}");
                    }
                case BindAction.Keys:
                    {
                        StringBuilder builder = new();
                        string[] keyNames = Enum.GetNames(typeof(Keys));
                        
                        for (int i = 0; i < keyNames.Length; i++)
                        {
                            string keyName = keyNames[i];
                            
                            if (i > 0)
                                builder.Append('\n');
                            builder.Append(keyName);
                        }

                        return builder.ToString();
                    }
                default:
                    throw new Exception($"Invalid action [{action.ToString()}]");
            }
        }

        public enum BindAction
        {
            Add,
            Remove,
            List,
            Keys
        }

        public record ConsoleBind(string hotkey, string command)
        {
            public string hotkey { get; set; } = hotkey;
            public string command { get; set; } = command;

            public override string ToString()
            {
                return $"[{hotkey}]({command})";
            }

            private static readonly Regex _parseRegex = new(@"\[.+\]\((.+)\)", RegexOptions.Compiled);

            public static ConsoleBind? Parse(string s)
            {
                Match m = _parseRegex.Match(s);

                return m.Success
                    ? new ConsoleBind(m.Groups[1].Value, m.Groups[2].Value)
                    : null;
            }

            public bool Activated => CustomKeyBinds.CheckInput(hotkey, CustomKeyBinds.CheckInputMethod.Pressed);

            public bool TryExecute()
            {
                try
                {
                    if (!Activated)
                        return false;
                }
                catch (Exception e)
                {
                    DevConsole.Log($"|DGRED|Error running bind: {e.Message}");
                    return false;
                }

                DevConsole.core.writeExecutedCommand = false;
                DevConsole.RunCommand(command);
                return true;
            }
        }
    }
}