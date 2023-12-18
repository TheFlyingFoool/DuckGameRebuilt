using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Exits the game")]
        public static void Exit()
        {
            MonoMain.exit = true;
        }
    }
}