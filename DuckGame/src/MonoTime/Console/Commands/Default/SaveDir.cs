using AddedContent.Firebreak;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Opens your game saves directory")]
        public static void SaveDir()
        {
            FNAPlatform.OpenURL(DuckFile.saveDirectory); // Process.Start
            DevConsole.Log(new DCLine
            {
                line = "Save directory was opened.",
                color = Color.White
            });
        }
    }
}