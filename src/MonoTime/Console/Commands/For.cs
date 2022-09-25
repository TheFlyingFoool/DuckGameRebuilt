namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Like the [rep] command but allows you to " +
                                         "declare a variable that'll be replaced with " +
                                         "the current execution cycle")]
        public static string For(string variableName, int times, string command)
        {
            for (int i = 0; i < times; i++)
            {
                DevConsole.RunCommand(command.Replace(variableName, i.ToString()), false);
            }

            return $"|DGBLUE|Repeated the command [{command}], [{times}] times!";
        }
    }
}
