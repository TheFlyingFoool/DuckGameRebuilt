namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Visualizes the hitboxes of things in the level")]
        public static bool ShowCollision()
        {
            return DevConsole.core.showCollision ^= true;
        }
    }
}