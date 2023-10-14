namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [DSHCommand(Description = "Closes MallardManager.")]
        public static void Close()
        {
            console.Active = false;
        }
    }
}