using System;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [AutoConfigField]
    public static int savethisfield = 2;
    
    [DevConsoleCommand]
    public static object Debug(int i, string arguments = "")
    {
        string[] args = arguments.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        switch (i)
        {
            case 0:
            {
                savethisfield = int.Parse(args[0]);
                break;
            }
        }

        return null;
    }
}