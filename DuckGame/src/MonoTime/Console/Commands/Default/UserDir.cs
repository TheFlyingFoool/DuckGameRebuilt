using AddedContent.Firebreak;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Opens your game user directory")]
        public static void UserDir()
        {
            FNAPlatform.OpenURL(DuckFile.userDirectory); // Process.Start
            DevConsole.Log(new DCLine
            {
                line = "User directory was opened.",
                color = Color.White
            });
        }
    }
}