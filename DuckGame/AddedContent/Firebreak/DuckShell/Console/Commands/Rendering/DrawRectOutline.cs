using AddedContent.Firebreak;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Renders a rectangle on the screen", To = ImplementTo.DuckShell)]
        public static void DrawRectOutline(Rectangle rectangle, Color color, Color outlineColor, float outlineSize, float depth, Layer layer)
        {
            s_drawRequests.TryAdd(layer.name, new Queue<DrawRequest>());
            
            s_drawRequests[layer.name].Enqueue(new DrawRequest(rectangle, color, depth).WithOutline(outlineColor, outlineSize));
        }
    }
}