using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Concatenates the provided items", To = ImplementTo.DuckShell)]
        public static string Join(params string[] items)
        {
            return JoinWith(string.Empty, items);
        }
    }
}