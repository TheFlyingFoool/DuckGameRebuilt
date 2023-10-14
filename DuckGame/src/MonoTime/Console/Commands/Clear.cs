using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Clears the console's text")]
        public static void Clear()
        {
            DevConsole.core.lines.Clear();
        }
    }
}