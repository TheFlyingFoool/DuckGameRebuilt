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
                return MemberAttributePairHandler.AttributeLookupRequests;
                break;
            }
        }

        return null;
    }
}