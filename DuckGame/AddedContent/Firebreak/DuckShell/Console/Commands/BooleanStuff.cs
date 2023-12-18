using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Name = "!", Description = "Returns true if false, and false if true", To = ImplementTo.DuckShell)]
        public static bool Invert(bool b) => !b;
    }
}