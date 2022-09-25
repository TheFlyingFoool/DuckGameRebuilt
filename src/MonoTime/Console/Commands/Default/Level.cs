namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(
            Name = "Level",
            Description = "Changes the current level",
            Aliases = new[] { "lev" })]
        public static void LevelCommand(Level level)
        {
            Level.current = level;
        }
    }
}