using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static object Debug(int i)
    {
        switch (i)
        {
            case 0:
            {
                return FireSerializer.Serialize(new[] {"x;y", "z"});
            }
            case 1:
            {
                return FireSerializer.Deserialize<string[]>(@"[x\;y;z]");
            }
            case 2:
            {
                return FireSerializer.Deserialize<string[]>(@"[x\;y\;z;w]");
            }
        }

        return null;
    }
}