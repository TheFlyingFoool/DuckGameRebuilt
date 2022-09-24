using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Toggles QWOP mode, similar to the modifier of the same name")]
        public static bool QwopMode()
        {
            return DevConsole.qwopMode ^= true;
        }
    }
}