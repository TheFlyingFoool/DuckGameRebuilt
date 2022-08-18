using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(Description = "Displays your current Frames Per Second for Duck Game")]
    public static bool Fps()
    {
        return DevConsole.showFPS ^= true;
    }
}