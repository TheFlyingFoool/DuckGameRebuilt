namespace DuckGame
{
    public class NMSetTeam : NMDuckNetwork
    {
        public Profile profile;
        public Team team;
        public bool custom;

        public NMSetTeam(Profile pProfile, Team pTeam, bool pCustomHat)
        {
            profile = pProfile;
            team = pTeam;
            custom = pCustomHat;
        }

        public NMSetTeam()
        {
        }
    }
}
