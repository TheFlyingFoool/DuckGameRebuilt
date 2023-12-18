using AddedContent.Firebreak;
using System.Diagnostics;
using System.Windows.Forms;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Restarts the game")]
        public static void Restart(string launchArgs = "*")
        {
            Process.Start(Application.ExecutablePath, launchArgs.Replace("*", Program.commandLine));
            Application.Exit();
            Program.main.KillEverything();
            Program.main.Exit();
        }
    }
}