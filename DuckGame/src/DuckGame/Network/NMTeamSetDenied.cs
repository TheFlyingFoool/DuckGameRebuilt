namespace DuckGame
{
    public class NMTeamSetDenied : NMDuckNetwork
    {
        public Profile profile;
        public Team team;

        public NMTeamSetDenied(Profile pProfile, Team pTeam)
        {
            profile = pProfile;
            team = pTeam;
        }

        public NMTeamSetDenied()
        {
        }
    }
}
