using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Runs a command in the DevConsole (DuckHack).", To = ImplementTo.DuckShell)]
        public static void DC(string command)
        {
            DevConsole.core.writeExecutedCommand = false;
            DevConsole.RunCommand(command);
        }
    }
}