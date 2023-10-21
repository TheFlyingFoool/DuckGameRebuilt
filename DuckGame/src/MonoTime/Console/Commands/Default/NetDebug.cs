using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Toggles network debugging mode", IsCheat = true)]
        public static bool NetDebug()
        {
            return DevConsole.EnableNetworkDebugging ^= true;
        }
    }
}