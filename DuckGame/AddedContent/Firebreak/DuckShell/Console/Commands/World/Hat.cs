using AddedContent.Firebreak;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Changes your hat to the selected hat")]
        public static void Hat([TeamAutoCompl] string teamName)
        {
            if (!DuckNetwork.active)
            {
                DevConsoleCommands.TeamCommand(Profiles.DefaultPlayer1, teamName);
                return;
            }

            Profile me = DuckNetwork.localProfile;
            DevConsoleCommands.TeamCommand(me, teamName);
            
            bool isCustomHat = me.team.customData is not null;
            Send.Message(new NMSetTeam(me, me.team, isCustomHat));
            if (isCustomHat)
                Send.Message(new NMSpecialHat(me.team, me, false));
        }
    }
}