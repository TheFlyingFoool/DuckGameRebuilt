using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Visualizes the center point of the current map")]
        public static bool ShowOrigin()
        {
            return DevConsole.debugOrigin ^= true;
        }
    }
}