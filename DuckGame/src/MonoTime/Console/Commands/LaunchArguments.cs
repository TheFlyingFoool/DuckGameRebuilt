using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(
            Description = "Prints your currently active program launch arguments",
            Aliases = new[] { "args", "commandline", "launchargs" })]
        public static string LaunchArguments()
        {
            return Program.commandLine;
        }
    }
}