namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Accepts something from a player (only god knows what)")]
        public static void Accept(Profile profile)
        {
            if (!DevConsole.core.transferRequestsPending.Contains(profile.connection))
                return;

            DevConsole.core.transferRequestsPending.Remove(profile.connection);
            DevConsole.SendNetLog(profile.connection);
        }
    }
}