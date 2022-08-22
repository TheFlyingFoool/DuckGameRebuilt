using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(
        Aliases = new[] { "8" },
        Description = "Fills all empty profile slots with a player")]
    public static void Eight()
    {
        for (int i = 0; i < Profiles.defaultProfiles.Count; i++)
        {
            Profile defaultProfile = Profiles.defaultProfiles[i];
                    
            // apparently the double assignment has purpose
            defaultProfile.team = null;
            defaultProfile.team = Teams.all[i];
        }
    }
}