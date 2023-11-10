using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Repeats the given input in the console",
            To = ImplementTo.DuckHack)]
        public static string Echo(string argument)
        {
            return argument;
        }
    }
}
