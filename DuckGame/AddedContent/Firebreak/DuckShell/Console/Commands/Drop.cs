using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Drops the given argument.", To = ImplementTo.DuckShell)]
        public static void Drop(string value)
        { }
    }
}