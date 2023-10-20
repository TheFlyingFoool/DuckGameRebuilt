using AddedContent.Firebreak;
using System;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Name = "?", Description = "Executes a command based on the condition.", To = ImplementTo.DuckShell)]
        public static object Conditional(bool condition, [CommandAutoCompl(false)] string commandIfTrue, params string[] alternative)
        {
            if (condition && !string.IsNullOrEmpty(commandIfTrue))
            {
                return Exec(commandIfTrue);
            }

            if (condition || alternative.Length == 0)
                return null;
            
            // 0 - syntax
            // 1 - condition
            // 2 - command
            int expecting = 0;
            bool failed = false;

            foreach (string word in alternative)
            {
                switch (expecting)
                {
                    case 0:
                    {
                        expecting = word switch
                        {
                            ":?" => 1,
                            ":" => 2,
                            _ => throw new Exception("Invalid syntax: " + word)
                        };
                        
                        break;
                    }

                    case 1:
                    {
                        ITypeInterpreter booleanInterpreter = console.Shell.TypeInterpreterModulesMap[typeof(bool)];
                        failed = !(bool) booleanInterpreter.ParseString(word, typeof(bool), console.Shell).Unpack();

                        expecting = 2;

                        break;
                    }

                    case 2:
                    {
                        if (failed)
                        {
                            failed = false;
                            expecting = 0;
                            break;
                        }

                        return Exec(word);
                    }
                }
            }

            return null;
        }
    }
}