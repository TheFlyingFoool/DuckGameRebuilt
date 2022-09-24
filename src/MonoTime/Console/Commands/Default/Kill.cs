using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Kills a player")]
        public static void Kill(Duck duck)
        {
            duck.Kill(new DTIncinerate(null));
        }
    }
}