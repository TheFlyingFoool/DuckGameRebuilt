using System;
using System.Linq;
using DuckGame.AddedContent.Drake;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static string Echo(string argument)
    {
        return argument;
    }
}
