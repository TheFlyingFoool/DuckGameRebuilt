
namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [DSHCommand(Description = "Quits the game.")]
        public static void Exit()
        {
            DevConsoleCommands.Exit();
        }
    }
}