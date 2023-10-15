using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Returns the length of the provided string.", To = ImplementTo.DuckShell)]
        public static int Length(string s)
        {
            return s.Length;
        }
    }
}