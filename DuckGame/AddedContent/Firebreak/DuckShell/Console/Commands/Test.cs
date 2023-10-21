using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(DebugOnly = true, To = ImplementTo.DuckShell)]
        public static string Test()
        {
            return "This\nis\na\ntest";
        }
    }
}