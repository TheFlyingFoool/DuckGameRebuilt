namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Toggles shield mode. You now have health in Duck Game", IsCheat = true)]
        public static bool ShieldMode()
        {
            return DevConsole.shieldMode ^= true;
        }
    }
}