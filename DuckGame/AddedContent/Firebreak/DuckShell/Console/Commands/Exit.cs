
using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Quits the game.")]
        public static void Exit()
        {
            DevConsoleCommands.Exit();
        }
    }
}