using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(
        Description = "Changes the current level",
        Aliases = new[] { "lev" })]
    public static void Level(Level level)
    {
        DuckGame.Level.current = level;
    }
}