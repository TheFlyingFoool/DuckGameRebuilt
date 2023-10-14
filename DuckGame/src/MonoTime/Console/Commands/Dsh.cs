using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Runs a DSH command", DebugOnly = true)]
        public static void Dsh(string command)
        {
            Commands.console.Run(command, false);
        }
    }
}