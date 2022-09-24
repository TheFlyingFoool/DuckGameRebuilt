using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Visualizes the outer bounds of the current map")]
        public static bool ShowBounds()
        {
            return DevConsole.debugBounds ^= true;
        }
    }
}