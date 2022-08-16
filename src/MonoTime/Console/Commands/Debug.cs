using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(Description = "If this command still exists after release im gonna eat my shoes[")]
    public static object Debug(int i)
    {
        switch (i)
        {
            case 0:
            {
                ProgressValue val = 0;
                DevConsole.Log("> val++");
                DevConsole.Log(val);
                DevConsole.Log(val++);
                DevConsole.Log(val);
                DevConsole.Log("> ++val");
                val = 0;
                DevConsole.Log(val);
                DevConsole.Log(++val);
                DevConsole.Log(val);
                return null;
            }
        }

        return null;
    }
}