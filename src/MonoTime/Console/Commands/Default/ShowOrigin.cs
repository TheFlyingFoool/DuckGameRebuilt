using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Visualizes the center point of the current map")]
        public static bool ShowOrigin()
        {
            return DevConsole.debugOrigin ^= true;
        }
    }
}