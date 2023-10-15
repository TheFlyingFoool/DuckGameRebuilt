using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Concatenates the provided items with a specified joiner string", To = ImplementTo.DuckShell)]
        public static string JoinWith(string with, params string[] items)
        {
            return string.Join(with, items);
        }
    }
}