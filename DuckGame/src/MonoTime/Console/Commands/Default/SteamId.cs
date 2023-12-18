using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Prints your Steam ID")]
        public static string SteamId()
        {
            return $"Your steam ID is: {Profiles.experienceProfile.steamID}";
        }
    }
}