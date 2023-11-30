using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Toggles constant sync")]
        public static bool ConstantSync()
        {
            return DevConsole.core.constantSync ^= true;
        }
    }
}