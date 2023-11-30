using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Name = "Log", Description = "Logs something? (not even the devs know)")]
        public static void LogCommand(string? description = null)
        {
            DevConsole.LogEvent(description, DuckNetwork.localConnection);
        }
    }
}