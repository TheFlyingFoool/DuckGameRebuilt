using AddedContent.Firebreak;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(DebugOnly = true,
            To = ImplementTo.DuckHack)]
        public static void Debug()
        {
            DevConsole.Log("UwU");
        }
    }
}