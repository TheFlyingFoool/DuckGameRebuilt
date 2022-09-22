using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Prints your Steam ID")]
        public static string SteamId()
        {
            return $"Your steam ID is: {Profiles.experienceProfile.steamID}";
        }
    }
}