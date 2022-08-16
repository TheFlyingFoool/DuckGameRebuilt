using System;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(Description = "Exits the game")]
    public static void Exit()
    {
        MonoMain.exit = true;
    }
}