using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Clears the console's text",
            To = ImplementTo.DuckHack)]
        public static void Clear()
        {
            DevConsole.core.lines.Clear();
        }
    }
}