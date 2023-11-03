namespace DuckGame
{
    public class NMClientDisconnect : NMDuckNetwork
    {
        public string whom;
        public Profile profile;

        public NMClientDisconnect()
        {
        }

        public NMClientDisconnect(string who, Profile pProfile)
        {
            whom = who;
            profile = pProfile;
        }

        public NMClientDisconnect(string who, byte pProfile)
        {
            whom = who;
            profile = DuckNetwork.profiles[pProfile];
        }
    }
}
