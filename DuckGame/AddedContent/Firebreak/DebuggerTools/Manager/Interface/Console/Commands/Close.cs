using DuckGame.ConsoleInterface;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Closes MallardManager.")]
        public static void Close()
        {
            console.Active = false;
        }
    }
}