#nullable enable
using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;
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
        [return: PrintReadableCollection]
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
                            return $"|DGRED|Key [{hotkey}] is not valid hotkey. Try running `bind keys` for a list of usable keys,\n" +
                                   $"|DGRED|combine them with \"+\", invert check with \"!\" before key, and check if key is down with \"*\" before key.\n" +
                                   $"|DGRED|Ex. of valid input: \"F1+*F2+!F3+*!F4\".\n" +
                                   $"|DGRED|It checks that F1 was just pressed, F2 is down, F3 wasn't just pressed, and F4 is not down.";
                        
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
                                builder.Append(", ");
                            builder.Append(keyName);
                        }

                        return builder.ToString();
                    }
                case BindAction.Rebind:
                    {
                        if (hotkey is null)
                            throw new Exception("No index provided");

                        if (command is null)
                            throw new Exception("No new hotkey provided");

                        if (!int.TryParse(hotkey, out int index)
                            || index < 0
                            || index >= Binds.Count)
                            throw new Exception($"Cannot cast [{hotkey}] to a valid index");

                        if (!CustomKeyBinds.IsValidInput(command))
                            return $"|DGRED|Key [{command}] is not valid hotkey. Try running `bind keys` for a list of usable keys,\n" +
                                   $"|DGRED|combine them with \"+\", invert check with \"!\" before key, and check if key is down with \"*\" before key.\n" +
                                   $"|DGRED|Ex. of valid input: \"F1+*F2+!F3+*!F4\".\n" +
                                   $"|DGRED|It checks that F1 was just pressed, F2 is down, F3 wasn't just pressed, and F4 is not down.";

                        Binds[index] = new(command, Binds[index].command);

                        return $"|DGBLUE|Rebinded [{index}] to [{command}]";
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
            Keys,
            Rebind
        }

        public record ConsoleBind
        {
            public string hotkey { get; set; }
            public string command { get; set; }

            public List<string> hotkeys;

            public ConsoleBind(string hotkey, string command)
            {
                this.hotkey = hotkey;
                this.command = command;

                hotkeys = new(hotkey.Split('+'));
            }

            public override string ToString()
            {
                if (!(hotkeys.Count > 1)) 
                    return $"[{hotkey}]({command})";

                string result = "{";

                foreach (string s in hotkeys)
                {
                    result += $"{s},";
                }
                result += $"}}({command}";

                return result;
            }

            public bool Activated => CustomKeyBinds.CheckInput(hotkeys, CustomKeyBinds.CheckInputMethod.Pressed);

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