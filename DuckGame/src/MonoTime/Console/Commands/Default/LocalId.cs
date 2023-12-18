using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Prints your local Duck Game build ID")]
        public static string LocalId()
        {
            return $"Your local ID is: {DG.localID}";
        }
    }
}