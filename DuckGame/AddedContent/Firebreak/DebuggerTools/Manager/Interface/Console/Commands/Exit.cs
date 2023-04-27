
namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Quits the game.")]
        public static void Exit()
        {
            DevConsoleCommands.Exit();
        }
    }
}