using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public class CommandAutoCompl : AutoCompl
    {
        public readonly bool MereName;
        public CommandAutoCompl(bool mereName = true)
        {
            MereName = mereName;
        }
        
        public override IList<string> Get(string word)
        {
            if (!MereName)
                return Array.Empty<string>();

            return GetCommandNames();
        }

        public static IList<string> GetCommandNames()
        {
            List<string> suggestions = new();
            for (int i = 0; i < Commands.console.Shell.Commands.Count; i++)
            {
                Marker.DevConsoleCommandAttribute command = Commands.console.Shell.Commands[i];
                    
                suggestions.Add(command.Name);
                suggestions.AddRange(command.Aliases);
            }

            return suggestions;
        }
    }
}