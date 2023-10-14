using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Toggles fancy mode. You spawn with FancyShoes every round", IsCheat = true)]
        public static bool FancyMode()
        {
            return DevConsole.fancyMode ^= true;
        }
    }
}