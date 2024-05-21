using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Converts a byte to its ASCII representation", To = ImplementTo.DuckShell)]
        public static char Ascii(byte b)
        {
            return (char)b;
        }
    }
}