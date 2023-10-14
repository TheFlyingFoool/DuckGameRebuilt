using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Concatenates the provided items with a specified joiner string")]
        public static string JoinWith(string with, params string[] items)
        {
            return string.Join(with, items);
        }
    }
}