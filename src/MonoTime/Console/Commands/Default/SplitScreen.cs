using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Toggles Split-Screen")]
        public static bool SplitScreen()
        {
            return DevConsole.splitScreen ^= true;
        }
    }
}