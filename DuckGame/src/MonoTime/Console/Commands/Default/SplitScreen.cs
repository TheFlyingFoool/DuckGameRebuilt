namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Toggles Split-Screen", IsCheat = true)]
        public static bool SplitScreen()
        {
            return DevConsole.splitScreen ^= true;
        }
    }
}