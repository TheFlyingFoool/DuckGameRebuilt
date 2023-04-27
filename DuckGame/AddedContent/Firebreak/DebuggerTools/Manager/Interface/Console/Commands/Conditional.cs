using System;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Name = "?", Description = "Executes a command based on the condition.", Hidden = true)]
        public static void Conditional(string condition, string commandIfTrue, char @else, string commandIfFalse)
        {
            if (@else != ':')
                throw new Exception($"Invalid syntax: [{@else}] used instead of [:]");

            if (!bool.TryParse(condition.ToLower(), out bool boolVal))
                throw new Exception($"Not a condition: {condition}");

            console.ExecuteCommand(boolVal ? commandIfTrue : commandIfFalse);
        }
    }
}