using AddedContent.Firebreak;
using System;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Returns the current mouse position")]
        public static Vec2 MousePos(MousePositionDimension dimension = default)
        {
            return dimension switch
            {
                MousePositionDimension.World   => Mouse.positionScreen,
                MousePositionDimension.Screen  => Mouse.position,
                MousePositionDimension.Console => Mouse.positionConsole,
                _                              => throw new ArgumentOutOfRangeException(nameof(dimension), dimension, null)
            };
        }
        
        public enum MousePositionDimension
        {
            World,
            Screen,
            Console
        }
    }
}