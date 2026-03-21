using AddedContent.Firebreak;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Opens the AdvancedConfig folder",
            Aliases = new[] {"advconf"})]
        public static void AdvancedConfig()
        {
            FNAPlatform.OpenURL(Marker.AdvancedConfigAttribute.SaveDirPath); // Process.Start
            DevConsole.Log("|DGBLUE|Opened AdvancedConfig folder");
            DevConsole.Log("|DGBLUE|Refresh with F5 to load your changes");
        }
    }
}
