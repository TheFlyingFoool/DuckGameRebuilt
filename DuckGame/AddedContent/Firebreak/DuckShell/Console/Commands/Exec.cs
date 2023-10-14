namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [DSHCommand(Description = "Executes a string as a command")]
        public static object Exec(string commandString)
        {
            return console.Shell.Run(commandString).Unpack();
        }
    }
}