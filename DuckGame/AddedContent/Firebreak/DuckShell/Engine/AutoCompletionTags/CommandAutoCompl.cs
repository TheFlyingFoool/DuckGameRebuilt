using System;

namespace DuckGame.ConsoleEngine
{
    public class CommandAutoCompl : AutoCompl
    {
        public readonly bool MereName;
        public CommandAutoCompl(bool mereName = true)
        {
            MereName = mereName;
        }
        
        public override string[] Get(string word)
        {
            if (!MereName)
                return Array.Empty<string>();

            string[] options = new string[Commands.console.Shell.Commands.Count];

            for (int i = 0; i < options.Length; i++)
            {
                options[i] = Commands.console.Shell.Commands[i].Name;
            }

            return options;
        }
    }
}