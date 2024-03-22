using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        public enum MousePositionDimension
        {
            World,
            Screen,
            Console
        }

        [Marker.DevConsoleCommand(Description = "Returns mouse position", To = ImplementTo.DuckShell)]
        public static string MousePos(MousePositionDimension dimension = default)
        {
            Vec2 position = dimension switch
            {
                MousePositionDimension.World => Mouse.positionScreen,
                MousePositionDimension.Screen => Mouse.position,
                MousePositionDimension.Console => Mouse.positionConsole
            };

            return $"{position.x},{position.y}";
        }
    }
}