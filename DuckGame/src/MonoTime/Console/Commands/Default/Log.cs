using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Logs something? (not even the devs know)")]
        public static void Log(string? description = null)
        {
            DevConsole.LogEvent(description, DuckNetwork.localConnection);
        }
    }
}