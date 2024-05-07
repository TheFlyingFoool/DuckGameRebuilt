using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Executes a string as a command", To = ImplementTo.DuckShell)]
        public static object Exec([CommandAutoCompl] string commandString)
        {
            return console.Shell.Run(commandString).Unpack();
        }
    }
}