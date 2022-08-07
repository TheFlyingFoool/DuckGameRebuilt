using System;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static void Clear()
    {
        DevConsole.core.lines.Clear();
    }
}