namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Prints your current DGR's version details")]
        public static string Version()
        {
            return $"{Program.CURRENT_VERSION_ID_FORMATTED} [{Program.gitVersion.Replace("[Modified]", "")}]";
        }
    }
}