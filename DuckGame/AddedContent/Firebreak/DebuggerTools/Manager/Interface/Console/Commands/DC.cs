namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [MMCommand(Description = "Runs a command in the DevConsole (DuckHack).")]
        public static void DC(string command)
        {
            DevConsole.core.writeExecutedCommand = false;
            DevConsole.RunCommand(command);
        }
    }
}