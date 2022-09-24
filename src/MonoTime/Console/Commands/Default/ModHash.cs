using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Prints your mod hash")]
        public static void ModHash()
        {
            DevConsole.Log($"{Color.Red.ToDGColorString()}{ModLoader._modString}");
            DevConsole.Log($"{Color.Red.ToDGColorString()}{ModLoader.modHash}");
        }
    }
}