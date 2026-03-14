using AddedContent.Firebreak;
using System.Diagnostics;
using System.Reflection;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Restarts the game")]
        public static void Restart(string launchArgs = "*")
        {
            Process.Start(Assembly.GetEntryAssembly().Location, launchArgs.Replace("*", Program.commandLine));
            Program.main.KillEverything();
            Program.main.Exit();
        }
    }
}