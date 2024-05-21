using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Executes a string as a DuckHack command", To = ImplementTo.DuckShell)]
        public static void ExecLegacy(string commandString)
        {
            DGRSettings.UseDuckShell = false;
            DevConsole.core.writeExecutedCommand = false;
            DevConsole.RunCommand(commandString);
            DGRSettings.UseDuckShell = true;
        }
    }
}