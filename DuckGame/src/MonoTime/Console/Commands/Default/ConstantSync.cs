namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Toggles constant sync")]
        public static bool ConstantSync()
        {
            return DevConsole.core.constantSync ^= true;
        }
    }
}