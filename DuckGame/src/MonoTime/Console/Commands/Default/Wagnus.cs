using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Toggles guides in editor for Wagnus teleport ranges")]
        public static bool Wagnus()
        {
            return DevConsole.wagnusDebug ^= true;
        }
    }
}