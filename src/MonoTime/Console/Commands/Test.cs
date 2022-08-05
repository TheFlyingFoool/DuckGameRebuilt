using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static string Test(string argument = "None")
    {
        return argument;
    }
}