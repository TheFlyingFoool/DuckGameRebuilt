namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Toggles fancy mode. You spawn with FancyShoes every round")]
        public static bool FancyMode()
        {
            return DevConsole.fancyMode ^= true;
        }
    }
}