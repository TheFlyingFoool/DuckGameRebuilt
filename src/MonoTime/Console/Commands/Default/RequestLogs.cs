using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Request the logs of (god knows) from everyone in the lobby")]
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