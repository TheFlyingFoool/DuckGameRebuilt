using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static void Test(bool one, bool two = false, string three = "three")
    {
        if (one)
            DevConsole.Log("one");
        if (two)
            DevConsole.Log("two");
        if (string.IsNullOrEmpty(three))
            DevConsole.Log(three);
    }
}