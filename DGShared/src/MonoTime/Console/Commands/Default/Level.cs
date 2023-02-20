namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Name = "Level", Description = "Changes the current level", Aliases = new[] { "lev" })]
        public static void LevelCommand(Level level)
        {
            if (!DevConsole.CheckCheats())
            {
                Level.current = level;
            }
            else
            {
                DevConsole.Log("You can't do that here!", Color.Red);
            }
        }
    }
}