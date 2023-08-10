using System.Linq;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Checks if you qualify as a cheater for the developer console")]
        public static string CheckCheats()
        {
            if (Network.isActive && Network.connections.Count == 1)
                return "|DGBLUE|PASS | Sole online player";

            if (NetworkDebugger.enabled)
                return "|DGBLUE|PASS | NetworkDebugger.enabled";

            ulong[] specialUsers =
            {
                76561197996786074UL,
                76561198885030822UL,
                76561198416200652UL,
                76561198104352795UL,
                76561198114791325UL,
            };

            if (Steam.user is null)
                return "|DGBLUE|PASS | Steam inactive";
            
            if (specialUsers.Contains(Steam.user.id))
                return "|DGBLUE|PASS | Exempt by landon";

            if (Network.isActive)
                return "|RED|FAIL | Online with other players";

            if (Level.current is ChallengeLevel or ArcadeLevel)
                return "|RED|FAIL | Arcade";

            return "|DGBLUE|PASS | Passed all cheater checks";
        }
    }
}