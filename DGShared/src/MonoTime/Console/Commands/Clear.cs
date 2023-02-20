namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Clears the console's text")]
        public static void Clear(bool verbose = true)
        {
            DevConsole.core.lines.Clear();

            if (verbose)
                DevConsole.Log("|DGBLUE|CLER|DGGREEN| CONSOLE CLEARED SUCCESSFULLY");
        }
    }
}