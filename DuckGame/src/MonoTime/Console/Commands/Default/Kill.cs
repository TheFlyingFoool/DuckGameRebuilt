using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Kills a player", IsCheat = true)]
        public static void Kill(Duck duck)
        {
            duck.Kill(new DTIncinerate(null));
        }
    }
}