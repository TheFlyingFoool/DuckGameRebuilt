using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Returns a random integer within the given range", To = ImplementTo.DuckShell)]
        public static int RNG(int minimum, int maximum)
        {
            return Rando.Int(minimum, maximum);
        }
    }
}