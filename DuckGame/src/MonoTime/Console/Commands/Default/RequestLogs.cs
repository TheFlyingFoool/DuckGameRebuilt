using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Set your network status to await incoming netlogs")]
        public static void RequestLogs()
        {
            Send.Message(new NMRequestLogs());

            foreach (NetworkConnection connection in Network.connections)
            {
                DevConsole.core.requestingLogs.Add(connection);
            }

            DevConsole.SaveNetLog();
        }
    }
}