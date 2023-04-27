using DuckGame.ConsoleInterface;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand]
        public static void MMToggle() => MallardManager.Open ^= true;
    }
}