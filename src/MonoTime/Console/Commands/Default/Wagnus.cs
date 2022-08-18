using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(Description = "Toggles guides in editor for Wagnus teleport ranges")]
    public static bool Wagnus()
    {
        return DevConsole.wagnusDebug ^= true;
    }
}