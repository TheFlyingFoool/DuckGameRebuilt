using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Concatenates the provided items")]
        public static string Join(params string[] items)
        {
            return JoinWith(string.Empty, items);
        }
    }
}