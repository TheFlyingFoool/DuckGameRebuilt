using AddedContent.Firebreak;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Toggles whether or not a layer is visible. Some options include 'game', 'background', 'blocks' and 'parallax'")]
        public static bool Toggle(Layer layer)
        {
            return layer.visible ^= true;
        }
    }
}