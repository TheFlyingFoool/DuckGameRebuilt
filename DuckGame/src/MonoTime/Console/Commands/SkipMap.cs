using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Skips the current map", HostOnly = true)]
        public static void SkipMap()
        {
            GameMode.Skip();
        }
    }
}
