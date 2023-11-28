using AddedContent.Firebreak;
using System.Diagnostics;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Opens your game user directory")]
        public static void UserDir()
        {
            Process.Start(DuckFile.userDirectory);
            DevConsole.Log(new DCLine
            {
                line = "User directory was opened.",
                color = Color.White
            });
        }
    }
}