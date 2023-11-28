using AddedContent.Firebreak;
using System.Diagnostics;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Opens your game saves directory")]
        public static void SaveDir()
        {
            Process.Start(DuckFile.saveDirectory);
            DevConsole.Log(new DCLine
            {
                line = "Save directory was opened.",
                color = Color.White
            });
        }
    }
}