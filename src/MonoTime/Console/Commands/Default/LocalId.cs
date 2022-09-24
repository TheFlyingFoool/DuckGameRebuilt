using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Prints your local Duck Game build ID")]
        public static string LocalId()
        {
            return $"Your local ID is: {DG.localID}";
        }
    }
}