using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Toggles the random level editor")]
        public static bool RandomEdit()
        {
            return Editor.miniMode ^= true;
        }
    }
}