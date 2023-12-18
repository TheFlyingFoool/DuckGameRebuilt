using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Sends a player your netlogs if they've requested them")]
        public static void Accept(Profile profile)
        {
            if (!DevConsole.core.transferRequestsPending.Contains(profile.connection))
                return;

            DevConsole.core.transferRequestsPending.Remove(profile.connection);
            DevConsole.SendNetLog(profile.connection);
        }
    }
}