using DuckGame.ConsoleInterface.Panes;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        private static MMConsolePane console => MMConsolePane.CurrentExecutor;
    }
}