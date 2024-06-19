using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(DebugOnly = true, To = ImplementTo.DuckShell)]
        public static string Test()
        {
            Vec2 vector = (0, 1);
            return $"x: {vector.x}, y: {vector.y}";
        }
    }
}