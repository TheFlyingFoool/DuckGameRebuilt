using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand]
        public static void Close()
        {
            if (DGRSettings.UseDuckShell)
                Commands.console.Active ^= true;
            else DevConsole.core.open ^= true;
        }
    }
}