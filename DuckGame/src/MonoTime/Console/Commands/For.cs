using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Like the [rep] command but allows you to " +
                                                "declare a variable that'll be replaced with " +
                                                "the current execution cycle",
            To = ImplementTo.DuckHack)]
        public static string For(string variableName, int times, string command)
        {
            for (int i = 0; i < times; i++)
            {
                DevConsole.core.writeExecutedCommand = false;
                DevConsole.RunCommand(command.Replace(variableName, i.ToString()));
            }

            return $"|DGBLUE|Repeated the command [{command}], [{times}] times!";
        }
    }
}
