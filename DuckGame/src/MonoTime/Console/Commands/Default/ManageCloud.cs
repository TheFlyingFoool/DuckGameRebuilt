using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Manage which files get cloud-synced", IsCheat = true)]
        public static void ManageCloud()
        {
            (MonoMain.pauseMenu = new UICloudManagement(null)).Open();
        }
    }
}