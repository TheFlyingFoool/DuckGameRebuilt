namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(
            Description = "Prints your currently active program launch arguments",
            Aliases = new[] { "args", "commandline", "launchargs" })]
        public static string LaunchArguments()
        {
            return Program.commandLine;
        }
    }
}