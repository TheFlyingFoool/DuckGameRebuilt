using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Toggles the random level editor")]
        public static bool RandomEdit()
        {
            return Editor.miniMode ^= true;
        }
    }
}