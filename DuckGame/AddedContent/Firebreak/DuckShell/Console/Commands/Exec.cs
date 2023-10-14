using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Executes a string as a command")]
        public static object Exec(string commandString)
        {
            return console.Shell.Run(commandString).Unpack();
        }
    }
}