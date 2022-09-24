using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Logs something? (not even the devs know)")]
        public static void Log(string? description = null)
        {
            DevConsole.LogEvent(description, DuckNetwork.localConnection);
        }
    }
}