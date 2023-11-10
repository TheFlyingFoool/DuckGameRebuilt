using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;
using System;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Name = "Team", Description = "Switches the team of the player", IsCheat = true)]
        public static void TeamCommand(Profile player, [TeamAutoCompl] string teamName)
        {
            if (Teams.all.TryFirst(x => x.name.CaselessEquals(teamName), out Team team))
            {
                player.team = team;
            }
            else
            {
                throw new Exception($"Team not found: {teamName}");
            }
        }
    }
}