using AddedContent.Firebreak;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Name = "mpeditor", Description = "Opens the multiplayer level editor for everyone in the lobby", HostOnly = true)]
        public static void MPEditor()
        {
            if (!Network.isActive)
            {
                DevConsole.Log("You have to be in an online lobby to open the multiplayer editor!", Color.Red);
                return;
            }
            if (MPEditorNet.active)
            {
                DevConsole.Log("The multiplayer editor is already open!", Color.Red);
                return;
            }
            Level.current = new MultiplayerEditor(MultiplayerEditor.NewSession());
        }
    }
}
