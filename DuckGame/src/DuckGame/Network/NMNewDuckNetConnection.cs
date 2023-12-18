namespace DuckGame
{
    public class NMNewDuckNetConnection : NMDuckNetwork
    {
        public string identifier;
        public string name;
        public Team team;
        public byte flippers;
        public bool parentalControlsActive;
        public int flagIndex;
        public Profile profile;
        public ulong profileID;
        public NetIndex16 latestGhostIndex;
        public byte persona;

        public NMNewDuckNetConnection()
        {
        }

        public NMNewDuckNetConnection(
          Profile pProfile,
          string id,
          string duckName,
          Team varTeam,
          byte varFlippers,
          bool parentalControls,
          int varFlagIndex,
          ulong pProfileID,
          byte pPersona)
        {
            identifier = id;
            name = duckName;
            team = varTeam;
            flippers = varFlippers;
            parentalControlsActive = parentalControls;
            flagIndex = varFlagIndex;
            profile = pProfile;
            profileID = pProfileID;
            latestGhostIndex = pProfile.latestGhostIndex;
            persona = pPersona;
        }
    }
}
