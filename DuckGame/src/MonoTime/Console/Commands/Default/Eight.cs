using AddedContent.Firebreak;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(
            Aliases = new[] { "8" },
            Description = "Fills all empty profile slots with a player",
            IsCheat = true)]
        public static void Eight()
        {
            for (int i = 0; i < Profiles.defaultProfiles.Count; i++)
            {
                Profile defaultProfile = Profiles.defaultProfiles[i];

                // apparently the double assignment has purpose
                defaultProfile.team = null;
                defaultProfile.team = Teams.all[i];
            }
        }
    }
}