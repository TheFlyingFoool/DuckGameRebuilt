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

        [Marker.DevConsoleCommand(Description = "Adds fake chat messages from Player2, Player3 and Player4 for testing the chat. " +
                                                "Only shows on your screen, needs a lobby (host one on your own). delay is in seconds, 0 sends them all at once")]
        public static void TestChat(int count = 8, float delay = 0.75f)
        {
            if (!ChatBox.QueueTestMessages(count, delay))
            {
                DevConsole.Log("The chat only shows online, host or join a lobby first", Color.Red);
                return;
            }
            DevConsole.Log("Sending " + Maths.Clamp(count, 1, 200) + " fake chat messages", Color.White);
        }
    }
}
