using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(Description = "Repeats the given input in the console")]
    public static string Echo(string argument)
    {
        return argument;
    }
}
