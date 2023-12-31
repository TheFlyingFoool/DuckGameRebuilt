using AddedContent.Firebreak;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Renders a string of text on the screen", To = ImplementTo.DuckShell)]
        public static void DrawStringOutline(string text, Vec2 position, Color color, Color outlineColor, float scale, float depth, Layer layer)
        {
            s_drawRequests.TryAdd(layer.name, new Queue<DrawRequest>());
            
            s_drawRequests[layer.name].Enqueue(new DrawRequest(text, position, color, depth, scale).WithOutline(outlineColor));
        }
    }
}