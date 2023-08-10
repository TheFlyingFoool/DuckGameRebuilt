namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Skips the current map", HostOnly = true)]
        public static void SkipMap(string argument)
        {
            GameMode.Skip();
        }
    }
}
