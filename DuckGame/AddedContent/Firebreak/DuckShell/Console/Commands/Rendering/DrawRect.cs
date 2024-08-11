using AddedContent.Firebreak;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        private static Dictionary<string, Queue<DrawRequest>> s_drawRequests = new();
        
        [Marker.DevConsoleCommand(Description = "Renders a rectangle on the screen", To = ImplementTo.DuckShell)]
        public static void DrawRect(Rectangle rectangle, Color color, float depth, Layer layer)
        {
            s_drawRequests.TryAdd(layer.name, new Queue<DrawRequest>());
            
            s_drawRequests[layer.name].Enqueue(new DrawRequest(rectangle, color, depth));
        }

        [Marker.ComplexDrawingContext]
        private static void HandleCommandDraws(Layer layer)
        {
            s_drawRequests.TryAdd(layer.name, new Queue<DrawRequest>());
            
            while (s_drawRequests[layer.name].Count > 0)
            {
                DrawRequest request = s_drawRequests[layer.name].Dequeue();

                if (request.Text is null)
                {
                    Graphics.DrawRect(request.Rectangle, request.Color, request.Depth);
                    
                    if (request.OutlineSize > 0)
                        Graphics.DrawRect(request.Rectangle.Shrink(-request.OutlineSize), request.OutlineColor, request.Depth, false, request.OutlineSize);
                }
                else
                {
                    if (request.OutlineSize > 0)
                    {
                        Graphics.DrawStringOutline(request.Text, request.Rectangle.tl, request.Color, request.OutlineColor, request.Depth, scale: request.Scale);
                    }
                    else
                    {
                        Graphics.DrawString(request.Text, request.Rectangle.tl, request.Color, request.Depth, scale: request.Scale);
                    }
                }
            }
        }

        private class DrawRequest
        {
            public float Depth;
            public Rectangle Rectangle;
            public string Text;
            public float Scale;
            public Color Color;
            public float OutlineSize;
            public Color OutlineColor;

            public DrawRequest(Rectangle rectangle, Color color, float depth)
            {
                Depth = depth;
                Rectangle = rectangle;
                Color = color;
            }

            public DrawRequest(string text, Vec2 position, Color color, float depth, float scale)
            {
                Text = text;
                Color = color;
                Depth = depth;
                Scale = scale;
                Rectangle = new Rectangle(position, position); // he's so real for that
            }

            public DrawRequest WithOutline(Color color, float size = 1f)
            {
                OutlineColor = color;
                OutlineSize = size;
                
                return this;
            }
        }
    }
}