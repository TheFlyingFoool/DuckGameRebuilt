using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Closes MallardManager.", To = ImplementTo.DuckShell)]
        public static void Close()
        {
            console.Active = false;
        }
    }
}