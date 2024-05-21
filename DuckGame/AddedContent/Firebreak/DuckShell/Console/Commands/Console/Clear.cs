using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Clears the console.", To = ImplementTo.DuckShell)]
        public static void Clear()
        {
            console.Clear();
        }
    }
}