using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Toggles Split-Screen", IsCheat = true)]
        public static bool SplitScreen()
        {
            return DevConsole.splitScreen ^= true;
        }
    }
}