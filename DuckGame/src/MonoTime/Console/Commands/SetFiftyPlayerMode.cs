using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(To = ImplementTo.DuckHack)]
        public static void SetFiftyPlayerMode(bool mood, int playerCount)
        {
            DG.FiftyPlayerMode = mood;
            DG.ExtraPlayerCount = playerCount;
        }
    }
}