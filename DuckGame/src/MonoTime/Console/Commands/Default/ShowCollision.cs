using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Visualizes the hitboxes of things in the level", IsCheat = true)]
        public static bool ShowCollision()
        {
            return DevConsole.core.showCollision ^= true;
        }
    }
}