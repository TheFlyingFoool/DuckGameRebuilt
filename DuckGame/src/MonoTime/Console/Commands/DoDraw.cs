using AddedContent.Firebreak;
using AddedContent.Hyeve.PolyRender;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Toggle the activity of a Drawing Context from it's ID")]
        public static string DoDraw(string id)
        {
            foreach (Marker.DrawingContextAttribute Attribute in Marker.DrawingContextAttribute.AllDrawingContexts)
            {
                string lookupName = Attribute.CustomID ?? Attribute.Name;

                if (!lookupName.CaselessEquals(id))
                    continue;

                bool prevState = Attribute.DoDraw;
                Attribute.DoDraw = !Attribute.DoDraw;
                return $"|DGBLUE|Drawing Context [{id}] toggled ({prevState} -> {!prevState})";
            }

            return $"|DGRED|No Drawing Context matches id of [{id}]";
        }

        [Marker.DrawingContext(DoDraw = false)]
        public static void TestDraw()
        {
            const float length = 8f;
            Color[] colors =
            {
                new(255, 000, 000),
                new(000, 255, 000),
                new(000, 000, 255),
                new(255, 255, 000),
            };
            Vec2 center = new(length, length);

            PolyRenderer.Tri(center, new Vec2(center.x + length, center.y), new Vec2(center.x, center.y + length), colors[0]);
            PolyRenderer.Tri(center, new Vec2(center.x - length, center.y), new Vec2(center.x, center.y), colors[1]);
            PolyRenderer.Tri(center, new Vec2(center.x, center.y), new Vec2(center.x, center.y - length), colors[2]);
            PolyRenderer.Tri(center, new Vec2(center.x - length, center.y), new Vec2(center.x, center.y - length), colors[3]);
        }
    }
}