using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Displays your current Frames Per Second for Duck Game")]
        public static bool Fps()
        {
            return DevConsole.showFPS ^= true;
        }
    }
}