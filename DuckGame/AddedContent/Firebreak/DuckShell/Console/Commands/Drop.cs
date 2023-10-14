using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Drops the given argument.")]
        public static void Drop(string value)
        { }
    }
}