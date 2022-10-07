namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Repeat a command multiple times")]
        public static string Rep(int times, string command)
        {
            for (int i = 0; i < times; i++)
            {
                DevConsole.core.writeExecutedCommand = false;
                DevConsole.RunCommand(command);
            }

            return $"|DGBLUE|Repeated the command [{command}], [{times}] times!";
        }
    }
}
