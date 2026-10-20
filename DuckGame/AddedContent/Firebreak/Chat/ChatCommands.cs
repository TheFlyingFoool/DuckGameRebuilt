using AddedContent.Firebreak;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Clears the online chat (only for you)")]
        public static void ClearChat()
        {
            int count = ChatBox.ClearChat();
            DevConsole.Log(count == 1 ? "Cleared 1 chat message" : "Cleared " + count + " chat messages", Color.White);
        }
    }
}
