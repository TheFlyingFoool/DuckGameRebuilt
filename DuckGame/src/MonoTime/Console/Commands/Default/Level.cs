using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Name = "Level", Description = "Changes the current level", Aliases = new[] { "lev" })]
        public static void Level(Level level)
        {
            DuckGame.Level.current = level;
        }
    }
}