
using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Quits the game.", To = ImplementTo.DuckShell)]
        public static void Exit()
        {
            DevConsoleCommands.Exit();
        }
    }
}