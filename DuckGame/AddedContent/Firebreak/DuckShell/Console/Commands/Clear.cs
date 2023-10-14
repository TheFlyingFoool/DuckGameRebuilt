using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DSHCommand(Description = "Clears the console.")]
        public static void Clear()
        {
            console.Clear();
        }
    }
}